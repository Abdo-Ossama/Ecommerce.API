namespace Ecommerce.API.DTOs.Requests
{
    public class UserEditRequest
    {
       
            public string ?UserName { get; set; }
            public string ?Email { get; set; }
            public string ?Fname { get; set; }
            public string ?Lname { get; set; }
            public string ?Address { get; set; }
            public string ? OldPassword { get; set; } 
            public string ? NewPassword { get; set; } 
        }
    }

