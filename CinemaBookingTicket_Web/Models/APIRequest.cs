

using Utility;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        public Constant.ApiType ApiType { get; set; } = Constant.ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }
    }
}
