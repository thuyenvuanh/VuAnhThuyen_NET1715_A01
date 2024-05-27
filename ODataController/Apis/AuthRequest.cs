using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Api_OData.Apis
{
    public class AuthRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public bool isAdmin (IConfiguration configuration)
        {
            string Email = configuration["AdminAccount:Email"];
            string Password = configuration["AdminAccount:Password"];
            return Email.Equals(this.Email) && Password.Equals(this.Password);
        }
    }
}
