namespace API.DTOs.Request;

public class VNPayCallbackDTO
{
    public long vnp_Amount { get; set; }
    public string vnp_BankCode { get; set; } = string.Empty;
    public string vnp_BankTranNo { get; set; } = string.Empty;
    public string vnp_CardType { get; set; } = string.Empty;
    public string vnp_OrderInfo { get; set; } = string.Empty;
    public string vnp_PayDate { get; set; } = string.Empty;
    public string vnp_ResponseCode { get; set; } = string.Empty;
    public string vnp_TmnCode { get; set; } = string.Empty;
    public string vnp_TransactionNo { get; set; } = string.Empty;
    public string vnp_TransactionStatus { get; set; } = string.Empty;
    public string vnp_TxnRef { get; set; } = string.Empty;
    public string vnp_SecureHash { get; set; } = string.Empty;

    // Chuyển đổi sang Dictionary<string, string> để tương thích với code hiện tại
    public Dictionary<string, string> ToDictionary()
    {
        var dict = new Dictionary<string, string>
        {
            { "vnp_Amount", vnp_Amount.ToString() },
            { "vnp_BankCode", vnp_BankCode },
            { "vnp_BankTranNo", vnp_BankTranNo },
            { "vnp_CardType", vnp_CardType },
            { "vnp_OrderInfo", vnp_OrderInfo },
            { "vnp_PayDate", vnp_PayDate },
            { "vnp_ResponseCode", vnp_ResponseCode },
            { "vnp_TmnCode", vnp_TmnCode },
            { "vnp_TransactionNo", vnp_TransactionNo },
            { "vnp_TransactionStatus", vnp_TransactionStatus },
            { "vnp_TxnRef", vnp_TxnRef },
            { "vnp_SecureHash", vnp_SecureHash }
        };

        // Loại bỏ các giá trị trống
        var emptyKeys = dict.Where(pair => string.IsNullOrEmpty(pair.Value)).Select(pair => pair.Key).ToList();
        foreach (var key in emptyKeys)
        {
            dict.Remove(key);
        }

        return dict;
    }
}
