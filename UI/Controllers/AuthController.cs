using Api_OData.Apis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using UI.Models;

namespace UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient client;
        private string AuthUrl = "https://localhost:7139/Auth";
        private IConfiguration configuration;

        public AuthController()
        {
            client = new();
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public IActionResult Login(string errorResponse)
        {
            ViewData["loginError"] = errorResponse;

            HttpContext.Session.Remove("MemberId");
            HttpContext.Session.Remove("JwtToken");
            return View();
        }

        [AllowAnonymous]
        [HttpGet("/Auth/Auth")]
        public IActionResult Login()
        {
            //init connection
            client.GetAsync(AuthUrl);
            return View();
        }

        [AllowAnonymous]
        [HttpPost("/Auth/Auth")]
        public IActionResult Login(AuthRequest request)
        {
            if (request == null) return BadRequest();

            string url = AuthUrl + "/Login";
            string strData = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(strData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Auth", new { errorResponse = response });
            }
            string jwtToken = response.Content.ReadAsStringAsync().Result;

            HttpContext.Session.SetString("MemberId", request.Email.ToString());
            HttpContext.Session.SetString("JwtToken", jwtToken);

            if (request.isAdmin(configuration))
            {
                return RedirectToAction("Index", "Reservation");
            }
            else
            {
                return RedirectToAction("Index", "Profile");
            }
        }

        [HttpPost("/Auth/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("MemberId");
            HttpContext.Session.Remove("JwtToken");
            return RedirectToAction("Login", "Auth");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
