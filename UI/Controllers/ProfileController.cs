using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Repository.Models;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace UI.Controllers
{
    [Route("Profile")]
    public class ProfileController : Controller
    {
        private HttpClient client;
        private const string reservationsApiUrl = "https://localhost:7139/odata/Reservations";
        private const string customerApi = "https://localhost:7139/odata/Customers";

        public ProfileController()
        {
            client = new();
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index(string errorResponse, string q = "")
        {
            HttpResponseMessage searchResponse = null;
            try
            {
                prepareToken();

                //TODO - query reservations
                string url = reservationsApiUrl +
                    $"?$expand={nameof(Reservation.Room)}($select={nameof(Room.Number)})" +
                    $"&$expand={nameof(Reservation.Customer)}($select={nameof(Customer.FullName)})";

                searchResponse = await client.GetAsync(url);
                searchResponse.EnsureSuccessStatusCode();

                string search = await searchResponse.Content.ReadAsStringAsync();
                dynamic tempSearch = JObject.Parse(search);
                List<Reservation> searchItems = ((JArray)tempSearch.value).Select(x => new Reservation
                {
                    Id = (int)x["Id"],
                    OnDate = (DateTime)x["OnDate"],
                    RoomId = (int)x["RoomId"],
                    Room = JsonConvert.DeserializeObject<Room>(x["Room"].ToString()),
                    Status = (string)x["Status"],
                    CustomerId = (int)x["CustomerId"],
                    Customer = JsonConvert.DeserializeObject<Customer>(x["Customer"].ToString())
                }).ToList();

                ViewData["q"] = q;
                ViewData["error"] = errorResponse;
                return View(searchItems);
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Auth", new { errorResponse = "You are not allowed to access this function." });
            }
        }

        [HttpGet("Details")]
        public async Task<IActionResult> Details()
        {
            //get user profile
            prepareToken();

            string url = $"{customerApi}" +
                $"?$select=Id,FullName,Telephone,Email,Birthday";

            var response = client.GetAsync(url).Result;
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Auth", new { errorResponse = "You are not allowed to access this function." });
            }

            var jsonProfile = await response.Content.ReadAsStringAsync();
            var profile = JsonConvert.DeserializeObject<Customer>(jsonProfile);
            return View(profile);
        }

        private string TrimJWTString(string jwtToken)
        {
            if (jwtToken != null)
            {
                // Remove '\"' in the token string
                jwtToken = jwtToken.Trim('\"');
                return jwtToken;
            }
            return "";
        }

        private void prepareToken()
        {
            var jwtToken = HttpContext.Session.GetString("JwtToken");
            jwtToken = TrimJWTString(jwtToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }
    }
}
