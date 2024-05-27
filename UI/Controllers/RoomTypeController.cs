using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Repository.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json.Converters;
using System.Text;

namespace UI.Controllers
{
    [Route("RoomType")]
    public class RoomTypeController : Controller
    {
        private readonly HttpClient client;
        private const string roomTypeApi = "https://localhost:7139/odata/RoomTypes";

        public RoomTypeController()
        {
            client = new HttpClient();
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index(string errorResponse, string q = "")
        {
            HttpResponseMessage searchResponse = null;
            try
            {
                prepareToken();

                //TODO - query reservations
                string url = roomTypeApi;

                searchResponse = await client.GetAsync(url);
                searchResponse.EnsureSuccessStatusCode();

                string search = await searchResponse.Content.ReadAsStringAsync();
                dynamic tempSearch = JObject.Parse(search);
                List<RoomType> searchItems = ((JArray)tempSearch.value).Select(x => new RoomType
                {
                    Id = (int)x["Id"],
                    Description = (string)x["Description"],
                    Name = (string)x["Name"],
                    Note = (string)x["Note"]
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

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RoomType roomType)
        {
            prepareToken();

            string jsonRoomType = JsonConvert.SerializeObject(roomType, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });
            StringContent content = new StringContent(jsonRoomType, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(roomTypeApi, content).Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "RoomType", new { errorResponse = response.ReasonPhrase });
            }

            return RedirectToAction("Index");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            prepareToken();

            //query details
            string url = $"{roomTypeApi}/{id}";

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "RoomType", new { errorResponse = response });
            }

            var strData = await response.Content.ReadAsStringAsync();
            var roomType = JsonConvert.DeserializeObject<RoomType>(strData);
            return View(roomType);
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            prepareToken();

            //query details
            string url = $"{roomTypeApi}/{id}";

            prepareToken();

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "RoomType", new { errorResponse = response });
            }

            var jsonRoomType = await response.Content.ReadAsStringAsync();
            var roomType = JsonConvert.DeserializeObject<RoomType>(jsonRoomType);
            return View(roomType);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RoomType roomType)
        {
            prepareToken();

            string jsonRoomType = JsonConvert.SerializeObject(roomType, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });
            StringContent content = new StringContent(jsonRoomType, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PatchAsync(roomTypeApi, content).Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "RoomType", new { errorResponse = response.ReasonPhrase });
            }

            return RedirectToAction("Index");
        }

        [HttpGet("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            prepareToken();

            HttpResponseMessage response = await client.GetAsync($"{roomTypeApi}/{id}");
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "RoomType", new { errorResponse = response });
            }

            string jsonRoomType = await response.Content.ReadAsStringAsync();
            var roomType = JsonConvert.DeserializeObject<RoomType>(jsonRoomType);
            return View(roomType);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(RoomType roomType)
        {
            prepareToken();

            var url = $"{roomTypeApi}/{roomType.Id}";

            HttpResponseMessage response = client.DeleteAsync(url).Result;
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "RoomType", new { errorResponse = response });
            }

            return RedirectToAction("Index");
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

        public IActionResult Index()
        {
            return View();
        }
    }
}
