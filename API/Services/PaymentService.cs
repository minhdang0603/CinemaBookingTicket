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

    }

    #region VNPay implementation

    public string CreateVNPayPaymentUrl(VNPayRequestDTO request)
    {
        string paymentUrl = VNPayHelper.CreatePaymentUrl(
            amount: request.Amount,
            orderInfo: request.OrderInfo,
            ipAddress: request.ClientIpAddress,
            returnUrl: request.ReturnUrl,
            tmnCode: _vnpayTmnCode,
            hashSecret: _vnpayHashSecret,
            baseUrl: _vnpayUrl,
            ipnUrl: _vnpayIpnUrl
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
            string orderId = queryParams["vnp_TxnRef"];
            string vnp_ResponseCode = queryParams["vnp_ResponseCode"];
            string vnp_TransactionStatus = queryParams["vnp_TransactionStatus"];
            string vnpayTranId = queryParams["vnp_TransactionNo"];

            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                // Payment successful - update database
                await UpdatePaymentStatusAsync(int.Parse(orderId), "Completed", vnpayTranId);

                return new VNPayIPNResponseDTO
                {
                    RspCode = "00",
                    Message = "Confirm Success"
                };
            }
            else
            {
                // Payment failed - update database
                await UpdatePaymentStatusAsync(int.Parse(orderId), "Failed", vnpayTranId);

                return new VNPayIPNResponseDTO
                {
                    RspCode = "00",
                    Message = "Confirm Success"
                };
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

    public async Task<VNPayResponseDTO> ProcessVNPayReturnAsync(IQueryCollection queryParams)
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
            string orderId = queryParams["vnp_TxnRef"];
            string vnp_ResponseCode = queryParams["vnp_ResponseCode"];
            string vnp_TransactionStatus = queryParams["vnp_TransactionStatus"];
            string vnpayTranId = queryParams["vnp_TransactionNo"];
            string amount = queryParams["vnp_Amount"];
            string payDate = queryParams["vnp_PayDate"];

            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                return new VNPayResponseDTO
                {
                    Success = true,
                    OrderId = orderId,
                    TransactionId = vnpayTranId,
                    Amount = decimal.Parse(amount) / 100,
                    ResponseCode = vnp_ResponseCode,
                    TransactionStatus = vnp_TransactionStatus,
                    TransactionDate = DateTime.ParseExact(payDate, "yyyyMMddHHmmss", null),
                    Message = "Payment successful! Your booking is confirmed.",
                    RedirectUrl = $"/booking-success/{orderId}"
                };
            }
            else
            {
                return new VNPayResponseDTO
                {
                    Success = false,
                    OrderId = orderId,
                    ResponseCode = vnp_ResponseCode,
                    TransactionStatus = vnp_TransactionStatus,
                    Message = GetVNPayResponseMessage(vnp_ResponseCode),
                    RedirectUrl = $"/booking-failed/{orderId}"
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

    public async Task<PaymentDTO> UpdatePaymentStatusAsync(int paymentId, string status, string transactionId = null)
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
            throw new AppException(ErrorCodes.InternalServerError());
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