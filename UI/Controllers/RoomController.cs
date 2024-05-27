using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Repository.Models;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UI.Controllers
{
    [Route("Room")]
    public class RoomController: Controller
    {
        private readonly HttpClient client;
        private const string roomApi = "https://localhost:7139/odata/Rooms";
        private const string roomTypeApi = "https://localhost:7139/odata/RoomTypes";

        public RoomController()
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
                string url = roomApi + 
                    $"?$filter=(Status eq false)" +
                    $"&$expand={nameof(Room.Type)}($select={nameof(RoomType.Name)})";

                searchResponse = await client.GetAsync(url);
                searchResponse.EnsureSuccessStatusCode();

                string search = await searchResponse.Content.ReadAsStringAsync();
                dynamic tempSearch = JObject.Parse(search);
                List<Room> searchItems = ((JArray)tempSearch.value).Select(x => new Room
                {
                    Id = (int)x["Id"],
                    Description = (string)x["Description"],
                    MaxCapacity = (int)x["MaxCapacity"],
                    Number = (int)x["Number"],
                    PricePerDate = (int)x["PricePerDate"],
                    Status = (bool)x["Status"],
                    Type = JsonConvert.DeserializeObject<RoomType>(x["Type"].ToString()),
                    TypeId = (int)x["TypeId"]
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
            prepareToken();
            //get available room types
            HttpResponseMessage roomTypeRes = client.GetAsync(roomTypeApi).Result;
            var roomJson = roomTypeRes.Content.ReadAsStringAsync().Result;
            // Parse odata roomTypeRes to List
            var rooms = JsonConvert.DeserializeObject<dynamic>(roomJson);
            List<RoomType> roomTypeList = JsonConvert.DeserializeObject<List<RoomType>>(rooms.value.ToString());
            ViewData["TypeId"] = new SelectList(roomTypeList, "Id", "Name");

            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room room)
        {
            prepareToken();

            string jsonRoomType = JsonConvert.SerializeObject(room, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });
            StringContent content = new StringContent(jsonRoomType, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(roomApi, content).Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Room", new { errorResponse = response.ReasonPhrase });
            }

            return RedirectToAction("Index");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            prepareToken();
            //query details
            string url = $"{roomApi}/{id}";

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Room", new { errorResponse = response });
            }

            var strData = await response.Content.ReadAsStringAsync();
            var room = JsonConvert.DeserializeObject<Room>(strData);
            return View(room);
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            prepareToken();
            //get available rooms
            HttpResponseMessage roomsRes = client.GetAsync(roomTypeApi).Result;
            var roomJson = roomsRes.Content.ReadAsStringAsync().Result;
            // Parse odata roomTypeRes to List
            var rooms = JsonConvert.DeserializeObject<dynamic>(roomJson);
            List<RoomType> roomTypeList = JsonConvert.DeserializeObject<List<RoomType>>(rooms.value.ToString());
            ViewData["TypeId"] = new SelectList(roomTypeList, "Id", "Name");

            prepareToken();
            //query details
            string url = $"{roomApi}/{id}";

            prepareToken();

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Room", new { errorResponse = response });
            }

            var jsonRoom = await response.Content.ReadAsStringAsync();
            var room = JsonConvert.DeserializeObject<Room>(jsonRoom);
            return View(room);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Room room)
        {
            prepareToken();

            string jsonRoom = JsonConvert.SerializeObject(room, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });
            StringContent content = new StringContent(jsonRoom, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PatchAsync(roomApi, content).Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Room", new { errorResponse = response.ReasonPhrase });
            }

            return RedirectToAction("Index");
        }

        [HttpGet("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            prepareToken();

            HttpResponseMessage response = await client.GetAsync($"{roomApi}/{id}");
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Room", new { errorResponse = response });
            }

            string jsonRoom = await response.Content.ReadAsStringAsync();
            var room = JsonConvert.DeserializeObject<Room>(jsonRoom);
            return View(room);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Room room)
        {
            prepareToken();

            var url = $"{roomApi}/{room.Id}";

            HttpResponseMessage response = client.DeleteAsync(url).Result;
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Room", new { errorResponse = response });
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
    }
}
