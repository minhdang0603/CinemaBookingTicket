
using Utility;

namespace Web.Models
{
    public class APIRequest
    {
        public Constant.ApiType ApiType { get; set; } = Constant.ApiType.GET;
        public string Url { get; set; } = string.Empty;
        public object? Data { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
