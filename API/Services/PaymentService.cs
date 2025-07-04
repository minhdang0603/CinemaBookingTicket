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
    private readonly string _vnpayIpnUrl;
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
        _vnpayIpnUrl = _configuration["VNPay:IpnUrl"] ?? "";
        _vnpayReturnUrl = _configuration["VNPay:ReturnUrl"] ?? "";
        _frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5082";
    }

    #region VNPay implementation    
    public async Task<string> CreateVNPayPaymentUrl(VNPayRequestDTO request)
    {
        var payment = await CreatePaymentAsync(request.BookingId);

        string paymentUrl = VNPayHelper.CreatePaymentUrl(
            amount: request.Amount,
            orderInfo: request.OrderInfo,
            ipAddress: request.ClientIpAddress,
            returnUrl: _vnpayReturnUrl,
            tmnCode: _vnpayTmnCode,
            hashSecret: _vnpayHashSecret,
            baseUrl: _vnpayUrl,
            ipnUrl: _vnpayIpnUrl,
            txnRef: payment.Id.ToString() // Sử dụng payment.Id làm vnp_TxnRef
        );

        return paymentUrl;
    }

    public async Task<VNPayIPNResponseDTO> ProcessVNPayIPNAsync(IQueryCollection queryParams)
    {
        var responseData = new SortedList<string, string>();

        foreach (var queryParam in queryParams)
        {
            responseData.Add(queryParam.Key, queryParam.Value.ToString());
        }

        bool isValidSignature = VNPayHelper.ValidateResponse(
            responseData,
            _vnpayHashSecret
        );

        if (isValidSignature)
        {
            string vnp_TxnRef = queryParams["vnp_TxnRef"].ToString();
            string vnp_ResponseCode = queryParams["vnp_ResponseCode"].ToString();
            string vnp_TransactionStatus = queryParams["vnp_TransactionStatus"].ToString();
            string vnpayTranId = queryParams["vnp_TransactionNo"].ToString();

            // vnp_TxnRef là payment_id từ hệ thống của chúng ta
            int paymentId = int.Parse(vnp_TxnRef);

            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                // Payment successful - update database
                await UpdatePaymentStatusAsync(paymentId, Constant.Payment_Status_Completed, vnpayTranId);

                // Optionally, you can also update the booking status to confirmed here
                var payment = await _unitOfWork.Payment.GetAsync(p => p.Id == paymentId);
                if (payment != null)
                {
                    var booking = await _unitOfWork.Booking.GetAsync(b => b.Id == payment.BookingId);
                    if (booking == null || !payment.BookingId.HasValue)
                    {
                        throw new AppException(ErrorCodes.BookingNotFound(payment.BookingId ?? 0));
                    }

                    booking.BookingStatus = Constant.Booking_Status_Confirmed;
                    booking.LastUpdatedAt = DateTime.Now;
                    await _unitOfWork.Booking.UpdateAsync(booking);
                    await _unitOfWork.SaveAsync();

                    return new VNPayIPNResponseDTO
                    {
                        RspCode = "00",
                        Message = "Confirm Success"
                    };
                }
                else
                {
                    throw new AppException(ErrorCodes.PaymentNotFound(paymentId));
                }
            }
            else
            {
                return new VNPayIPNResponseDTO
                {
                    RspCode = "97",
                    Message = "Invalid Signature"
                };
            }
        }

        return new VNPayIPNResponseDTO
        {
            RspCode = "97",
            Message = "Invalid Signature"
        };
    }
    public async Task<VNPayResponseDTO> ProcessVNPayReturnAsync(IQueryCollection queryParams)
    {
        var responseData = new SortedList<string, string>();

        foreach (var queryParam in queryParams)
        {
            responseData.Add(queryParam.Key, queryParam.Value.ToString());
        }

        // Use await Task.Run to ensure this method is truly asynchronous
        bool isValidSignature = await Task.Run(() => VNPayHelper.ValidateResponse(
            responseData,
            _vnpayHashSecret
        ));

        if (isValidSignature)
        {
            string vnp_TxnRef = queryParams["vnp_TxnRef"].ToString();
            string vnp_ResponseCode = queryParams["vnp_ResponseCode"].ToString();
            string vnp_TransactionStatus = queryParams["vnp_TransactionStatus"].ToString();
            string vnpayTranId = queryParams["vnp_TransactionNo"].ToString();
            string amount = queryParams["vnp_Amount"].ToString();
            string payDate = queryParams["vnp_PayDate"].ToString();

            // Parse payment ID
            if (!int.TryParse(vnp_TxnRef, out int paymentId))
            {
                throw new AppException(ErrorCodes.InternalServerError());
            }

            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                // Update payment status to completed
                await UpdatePaymentStatusAsync(paymentId, Constant.Payment_Status_Completed, vnpayTranId);
                
                // Get payment and related booking information
                var payment = await _unitOfWork.Payment.GetAsync(p => p.Id == paymentId);
                if (payment != null && payment.BookingId.HasValue)
                {
                    var booking = await _unitOfWork.Booking.GetAsync(
                        b => b.Id == payment.BookingId && b.IsActive, 
                        includeProperties: "ShowTime,ShowTime.Movie,ApplicationUser");
                    
                    if (booking != null)
                    {
                        // Update booking status
                        booking.BookingStatus = Constant.Booking_Status_Confirmed;
                        booking.LastUpdatedAt = DateTime.Now;
                        await _unitOfWork.Booking.UpdateAsync(booking);
                        await _unitOfWork.SaveAsync();
                    }
                }

                // Get the frontend URL and success route from configuration
                string successRoute = _configuration["Frontend:SuccessRoute"] ?? "/payment/success/{0}";

                // Format the success route with the payment ID
                string formattedSuccessRoute = string.Format(successRoute, vnp_TxnRef);

                // Use the _frontendUrl for the RedirectUrl
                return new VNPayResponseDTO
                {
                    Success = true,
                    OrderId = payment.BookingId.ToString(),
                    TransactionId = vnpayTranId,
                    Amount = decimal.Parse(amount) / 100,
                    ResponseCode = vnp_ResponseCode,
                    TransactionStatus = vnp_TransactionStatus,
                    TransactionDate = DateTime.ParseExact(payDate, "yyyyMMddHHmmss", null),
                    Message = "Payment successful! Your booking is confirmed.",
                    RedirectUrl = $"{_frontendUrl.TrimEnd('/')}{formattedSuccessRoute}"
                };
            }
            else
            {
                // Get the frontend URL and failure route from configuration
                string failureRoute = _configuration["Frontend:FailureRoute"] ?? "/payment/failed/{0}";

                // Format the failure route with the payment ID
                string formattedFailureRoute = string.Format(failureRoute, vnp_TxnRef);

                // Use the _frontendUrl for the RedirectUrl
                return new VNPayResponseDTO
                {
                    Success = false,
                    OrderId = vnp_TxnRef,
                    ResponseCode = vnp_ResponseCode,
                    TransactionStatus = vnp_TransactionStatus,
                    Message = GetVNPayResponseMessage(vnp_ResponseCode),
                    RedirectUrl = $"{_frontendUrl.TrimEnd('/')}{formattedFailureRoute}"
                };
            }
        }
        else
        {
            throw new AppException(ErrorCodes.InternalServerError());
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
            throw new AppException(ErrorCodes.BookingNotFound(bookingId));
        }

        var payment = new Payment
        {
            BookingId = bookingId,
            Amount = booking.TotalAmount,
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
            throw new AppException(ErrorCodes.PaymentNotFound(bookingId));
        }

        return _mapper.Map<PaymentDTO>(payment);
    }
}