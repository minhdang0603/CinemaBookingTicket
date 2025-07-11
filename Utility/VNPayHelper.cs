using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Utility
{
    public static class VNPayHelper
    {
        public static string CreatePaymentUrl(decimal amount, string orderInfo, string ipAddress, string returnUrl, string tmnCode, string hashSecret, string baseUrl, string txnRef = null)
        {
            var vnpParams = new SortedList<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", ((int)(amount * 100)).ToString() },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", ipAddress },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_TxnRef", string.IsNullOrEmpty(txnRef) ? DateTime.Now.Ticks.ToString() : txnRef},
                { "vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss") }
            };

            // Generate Secure Hash
            var queryString = string.Join("&", vnpParams.Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));
            var secureHash = HmacSha512(hashSecret, queryString);

            // Append hash to the URL
            var paymentUrl = $"{baseUrl}?{queryString}&vnp_SecureHash={secureHash}";
            return paymentUrl;
        }

        public static string HmacSha512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static bool ValidateResponse(SortedList<string, string> responseData, string hashSecret)
        {
            if (!responseData.ContainsKey("vnp_SecureHash"))
            {
                return false; // Không có chữ ký bảo mật, xác thực thất bại
            }

            // Lấy chữ ký từ dữ liệu phản hồi
            string secureHash = responseData["vnp_SecureHash"];

            // Tạo bản sao của responseData để tránh thay đổi dữ liệu gốc
            var validationData = new SortedList<string, string>(responseData);
            validationData.Remove("vnp_SecureHash");

            // Loại bỏ vnp_SecureHashType nếu có
            if (validationData.ContainsKey("vnp_SecureHashType"))
            {
                validationData.Remove("vnp_SecureHashType");
            }

            // Tạo chuỗi query string để tính toán chữ ký
            // Sử dụng WebUtility.UrlEncode để phù hợp với cách mã hóa của VNPay
            var queryString = string.Join("&", validationData.Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            // Tính toán chữ ký bằng phương thức HmacSha512 - giống với phương thức tạo URL thanh toán
            var checkSum = HmacSha512(hashSecret, queryString);

            // So sánh chữ ký đã tính với chữ ký nhận được
            return checkSum.Equals(secureHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}