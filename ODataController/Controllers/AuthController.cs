using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Api_OData.Apis;
using Repository;
using Repository.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api_OData.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController: Controller
    {
        private readonly IConfiguration configuration;
        private readonly ICustomerRepository repository;

        public AuthController(IConfiguration configuration, ICustomerRepository repository)
        {
            this.configuration = configuration;
            this.repository = repository;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] AuthRequest authRequest)
        {
            Customer customer;

            bool isAdmin = authRequest.isAdmin(configuration);
            if (isAdmin)
            {
                customer = new()
                {
                    Email = authRequest.Email,
                    Password = authRequest.Password,
                };
            }
            else
            {
                var cust= repository.Login(authRequest.Email, authRequest.Password);
                if (cust == null) return Unauthorized();
                customer = cust;
            }

            var jwtToken = GenerateJwt(customer, isAdmin ? "Admin" : "Customer");
            return Ok(jwtToken);
        }

        private string GenerateJwt(Customer customer, string role)
        {
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimTypes.UserData, customer.Id.ToString()),
                        new Claim(ClaimTypes.Role, role)
                    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                                    configuration["Jwt:Issuer"],
                                    configuration["Jwt:Audience"],
                                    claims,
                                    expires: DateTime.UtcNow.AddMinutes(10),
                                    signingCredentials: signIn);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
