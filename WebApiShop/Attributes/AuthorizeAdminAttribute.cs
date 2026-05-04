using Microsoft.AspNetCore.Authorization;

namespace WebApiShop.Attributes
{
    public class AuthorizeAdminAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminAttribute()
        {
            Roles = "Admin";
        }
    }
}


