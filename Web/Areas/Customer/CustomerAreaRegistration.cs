using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Customer
{
    public class CustomerAreaRegistration : AreaAttribute
    {
        public CustomerAreaRegistration() : base("Customer")
        {
        }
    }
}
