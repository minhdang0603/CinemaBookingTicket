
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

        public enum AgeRatingType
        {
            G,
            PG,
            PG13,
            R,
            NC17
        }

        public const string SessionToken = "JWTToken";

        public const string Role_Customer = "Customer";
        public const string Role_Employee = "Employee";
        public const string Role_Admin = "Admin";

        public const string Movie_Status_ComingSoon = "coming soon";
        public const string Movie_Status_NowShowing = "now showing";

        public const string Payment_Status_Pending = "pending";
        public const string Payment_Status_Completed = "completed";
        public const string Payment_Status_Failed = "failed";
        public const string Payment_Status_Refunded = "refunded";

        public const string Booking_Status_Pending = "pending";
        public const string Booking_Status_Confirmed = "confirmed";
        public const string Booking_Status_Cancelled = "cancelled";
        public const string Booking_Status_Refunded = "refunded";

        public const string Seat_Type_Standard = "Standard";
        public const string Seat_Type_Premium = "Premium";
        public const string Seat_Type_VIP = "VIP";
    }
}
