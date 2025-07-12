using System.Threading.Tasks;
using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using Stripe;
using Utility;

namespace API.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    // VNPay configuration
    private readonly string _vnpayTmnCode;
    private readonly string _vnpayHashSecret;
    private readonly string _vnpayUrl;
    private readonly string _vnpayReturnUrl;
    private readonly string _frontendUrl;

    public PaymentService(IConfiguration configuration, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _mapper = mapper;

        // Initialize VNPay configuration
        _vnpayTmnCode = _configuration["VNPay:TmnCode"] ?? "";
        _vnpayHashSecret = _configuration["VNPay:HashSecret"] ?? "";
        _vnpayUrl = _configuration["VNPay:Url"] ?? "";
        _vnpayReturnUrl = _configuration["VNPay:ReturnUrl"] ?? "";
        _frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5082";
    }

    #region VNPay implementation    
    public async Task<string> CreateVNPayPaymentUrl(VNPayRequestDTO request)
    {
        var payment = await CreatePaymentAsync(request.BookingId);

        string paymentUrl = VNPayHelper.CreatePaymentUrl(
            amount: (long)request.Amount,
            orderInfo: request.OrderInfo,
            ipAddress: request.ClientIpAddress,
            returnUrl: _vnpayReturnUrl,
            tmnCode: _vnpayTmnCode,
            hashSecret: _vnpayHashSecret,
            baseUrl: _vnpayUrl,
            txnRef: payment.Id.ToString() // Sử dụng payment.Id làm vnp_TxnRef
        );

        return paymentUrl;
    }

    public async Task<VNPayResponseDTO> ProcessVNPayReturnAsync(Dictionary<string, string> queryParams)
    {
        var responseData = new SortedList<string, string>();

        foreach (var queryParam in queryParams)
        {
            responseData.Add(queryParam.Key, queryParam.Value);
        }

        // Kiểm tra chữ ký VNPay
        bool isValidSignature = VNPayHelper.ValidateResponse(responseData, _vnpayHashSecret);

        if (!isValidSignature)
        {
            throw new AppException(ErrorCodes.InternalServerError("Invalid VNPay signature"));
        }

        string vnp_TxnRef = queryParams["vnp_TxnRef"];
        string vnp_ResponseCode = queryParams["vnp_ResponseCode"];
        string vnp_TransactionStatus = queryParams["vnp_TransactionStatus"];
        string vnpayTranId = queryParams["vnp_TransactionNo"];
        string amount = queryParams["vnp_Amount"];
        string payDate = queryParams["vnp_PayDate"];

        // Parse payment ID
        if (!int.TryParse(vnp_TxnRef, out int paymentId))
        {
            throw new AppException(ErrorCodes.InternalServerError("Invalid payment reference"));
        }

        // Lấy thông tin payment
        var payment = await _unitOfWork.Payment.GetAsync(p => p.Id == paymentId);
        if (payment == null)
        {
            throw new AppException(ErrorCodes.EntityNotFound("Payment", paymentId));
        }

        // Create base response
        var response = CreateBaseVNPayResponse(payment, vnpayTranId, amount, vnp_ResponseCode, 
                                             vnp_TransactionStatus, payDate);

        // Determine if payment is successful
        bool isPaymentSuccessful = vnp_ResponseCode == "00" && vnp_TransactionStatus == "00";
        
        // Update payment status based on result
        string paymentStatus = isPaymentSuccessful 
            ? Constant.Payment_Status_Completed 
            : Constant.Payment_Status_Failed;
            
        // Update booking status based on payment result
        string bookingStatus = isPaymentSuccessful 
            ? Constant.Booking_Status_Confirmed 
            : Constant.Booking_Status_Cancelled;

        // Update payment and associated entities
        await UpdatePaymentDetails(payment, paymentStatus, vnpayTranId, isPaymentSuccessful);
        await UpdateBookingDetailsForResponse(payment, bookingStatus, response);
        
        // Complete response with appropriate messages and redirect URL
        CompleteVNPayResponse(response, isPaymentSuccessful, vnp_ResponseCode, paymentId);

        return response;
    }

    private void CompleteVNPayResponse(VNPayResponseDTO response, bool isPaymentSuccessful, string responseCode, int paymentId)
    {
        response.Success = isPaymentSuccessful;
        
        response.Message = isPaymentSuccessful 
            ? "Payment successful! Your booking is confirmed." 
            : GetVNPayResponseMessage(responseCode);
            
        string resultPath = isPaymentSuccessful ? "success" : "failed";
        response.RedirectUrl = $"{_frontendUrl.TrimEnd('/')}/payment/{resultPath}/{paymentId}";
    }

    private VNPayResponseDTO CreateBaseVNPayResponse(Payment payment, string transactionId, string amount, 
                                                    string responseCode, string transactionStatus, string payDate)
    {
        return new VNPayResponseDTO
        {
            OrderId = payment.BookingId?.ToString() ?? "0",
            TransactionId = transactionId,
            Amount = decimal.Parse(amount) / 100,
            ResponseCode = responseCode,
            TransactionStatus = transactionStatus,
            TransactionDate = DateTime.ParseExact(payDate, "yyyyMMddHHmmss", null),
        };
    }

    private async Task UpdatePaymentDetails(Payment payment, string status, string transactionId, bool isPaymentSuccessful)
    {
        payment.PaymentStatus = status;
        payment.TransactionId = transactionId;
        payment.LastUpdatedAt = DateTime.Now;
        
        if (isPaymentSuccessful)
        {
            payment.PaymentDate = DateTime.Now;
        }
        
        await _unitOfWork.Payment.UpdateAsync(payment);
    }

    private async Task UpdateBookingDetailsForResponse(Payment payment, string bookingStatus, VNPayResponseDTO response)
    {
        if (!payment.BookingId.HasValue)
        {
            return;
        }

        var booking = await _unitOfWork.Booking.GetAsync(
            b => b.Id == payment.BookingId && b.IsActive,
            includeProperties: "ShowTime,ShowTime.Movie,ApplicationUser,BookingDetails");

        if (booking == null)
        {
            return;
        }

        // Update booking status
        booking.BookingStatus = bookingStatus;
        booking.LastUpdatedAt = DateTime.Now;

        // Populate response with booking details
        UpdateResponseWithBookingDetails(response, booking);

        // Update booking in database
        await _unitOfWork.Booking.UpdateAsync(booking);
        response.Booking = _mapper.Map<BookingDTO>(booking);

        // Update related concession orders
        await UpdateConcessionOrders(payment.BookingId.Value, bookingStatus);
    }

    private void UpdateResponseWithBookingDetails(VNPayResponseDTO response, Booking booking)
    {
        response.BookingCode = booking.BookingCode;
        response.CustomerName = booking.ApplicationUser?.UserName ?? string.Empty;
        response.SeatNames = booking.BookingDetails?.Select(bd => bd.SeatName).ToList() ?? new List<string>();

        // Add showtime details if available
        if (booking.ShowTime != null)
        {
            response.MovieTitle = booking.ShowTime.Movie?.Title ?? string.Empty;
            response.TheaterName = booking.ShowTime.Screen?.Theater?.Name ?? string.Empty;
            response.ScreenName = booking.ShowTime.Screen?.Name ?? string.Empty;
            response.ShowDate = booking.ShowTime.ShowDate.ToDateTime(TimeOnly.MinValue);
            response.ShowTime = booking.ShowTime.StartTime;
        }
    }

    private async Task UpdateConcessionOrders(int bookingId, string status)
    {
        var concessionOrders = await _unitOfWork.ConcessionOrder.GetAllAsync(
            co => co.BookingId == bookingId && co.IsActive);

        foreach (var order in concessionOrders)
        {
            order.OrderStatus = status;
            order.LastUpdatedAt = DateTime.Now;
            await _unitOfWork.ConcessionOrder.UpdateAsync(order);
        }
    }

    private string GetVNPayResponseMessage(string responseCode)
    {
        return responseCode switch
        {
            "00" => "Transaction successful",
            "07" => "Deduct money successfully. Transaction is suspected of fraud",
            "09" => "Transaction failed: Customer's card/account not registered for internet banking service at the bank",
            "10" => "Transaction failed: Customer incorrectly verified transaction information more than 3 times",
            "11" => "Transaction failed: Payment timeout. Please try again",
            "12" => "Transaction failed: Customer's card/account is locked",
            "13" => "Transaction failed: Incorrect transaction authentication password",
            "24" => "Transaction failed: Customer canceled transaction",
            "51" => "Transaction failed: Customer's account has insufficient balance",
            "65" => "Transaction failed: Customer's account has exceeded daily limit",
            "75" => "Payment bank is under maintenance",
            "79" => "Transaction failed: Customer enters incorrect payment password more than allowed times",
            _ => "Transaction failed"
        };
    }
    #endregion

    public async Task<PaymentDTO> UpdatePaymentStatusAsync(int paymentId, string status, string? transactionId = null)
    {
        var payment = await _unitOfWork.Payment.GetAsync(p => p.Id == paymentId);

        if (payment == null)
        {
            throw new Exception("Payment not found");
        }

        payment.PaymentStatus = status;
        payment.TransactionId = transactionId;
        payment.LastUpdatedAt = DateTime.Now;

        if (status == Constant.Payment_Status_Completed)
        {
            payment.PaymentDate = DateTime.Now;
        }

        await _unitOfWork.Payment.UpdateAsync(payment);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<PaymentDTO>(payment);
    }

    public async Task<PaymentDTO> CreatePaymentAsync(int bookingId)
    {
        var booking = await _unitOfWork.Booking.GetAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            throw new AppException(ErrorCodes.EntityNotFound("Booking", bookingId));
        }

        var concessionOrders = await _unitOfWork.ConcessionOrder.GetAllAsync(co => co.BookingId == bookingId && co.IsActive);
        decimal totalAmount = booking.TotalAmount;
        
        if (concessionOrders != null && concessionOrders.Any())
        {
            totalAmount += concessionOrders.Sum(co => co.TotalAmount);
        }

        var payment = new Payment
        {
            BookingId = bookingId,
            Amount = totalAmount,
            PaymentDate = DateTime.Now,
            PaymentStatus = Constant.Payment_Status_Pending,
            CreatedAt = DateTime.Now,
            LastUpdatedAt = DateTime.Now
        };

        await _unitOfWork.Payment.CreateAsync(payment);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<PaymentDTO>(payment);
    }

    public async Task<PaymentDTO> GetPaymentsByBookingIdAsync(int bookingId)
    {
        var payment = await _unitOfWork.Payment.GetAsync(p => p.BookingId == bookingId);

        if (payment == null)
        {
            throw new AppException(ErrorCodes.EntityNotFound("Payment", bookingId));
        }

        return _mapper.Map<PaymentDTO>(payment);
    }
}