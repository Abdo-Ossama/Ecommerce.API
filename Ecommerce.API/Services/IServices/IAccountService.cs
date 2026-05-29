using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Services.IServices
{
    public interface IAccountService
    {
         Task sendEmailAsync(EmailType emailType, ApplicationUser applicationUser, string msg);
    }
}
