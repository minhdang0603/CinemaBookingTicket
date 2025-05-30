
namespace Utility
{
    public static class Constant
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public const string SessionToken = "JWTToken";

        public const string Role_Customer = "Customer";
        public const string Role_Employee = "Employee";
        public const string Role_Admin = "Admin";
    }
}
