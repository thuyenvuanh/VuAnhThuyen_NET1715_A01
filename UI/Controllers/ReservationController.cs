using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Repository.Models;

namespace UI.Controllers
{
    [Route("Reservation")]
    public class ReservationController : Controller
    {
        private readonly HttpClient client;
        private const string reservationsApiUrl = "https://localhost:7139/odata/Reservations";
        private const string roomApiUrl = "https://localhost:7139/odata/Rooms";
        private const string customerApiUrl = "https://localhost:7139/odata/Customers";

        public ReservationController()
        {
            this.client = new HttpClient();
        }
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

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            prepareToken();

            //query details
            string url = reservationsApiUrl + $"/{id}" +
                $"?$expand={nameof(Reservation.Room)}($select=Id,Number)" +
                $"&$expand={nameof(Reservation.Customer)}($select=Id,FullName)";

            prepareToken();

            //get available rooms
            HttpResponseMessage roomsRes = client.GetAsync(roomApiUrl).Result;
            var roomJson = roomsRes.Content.ReadAsStringAsync().Result;
            // Parse odata roomsRes to List
            var rooms = JsonConvert.DeserializeObject<dynamic>(roomJson);
            List<Room> roomList = JsonConvert.DeserializeObject<List<Room>>(rooms.value.ToString());
            ViewData["RoomId"] = new SelectList(roomList, "Id", "Number");

            prepareToken();
            //get available Customer
            HttpResponseMessage custRes = client.GetAsync(customerApiUrl).Result;
            var custJson = custRes.Content.ReadAsStringAsync().Result;
            // Parse odata roomsRes to List
            var customers = JsonConvert.DeserializeObject<dynamic>(custJson);
            List<Customer> customersList = JsonConvert.DeserializeObject<List<Customer>>(customers.value.ToString());
            ViewData["CustomerId"] = new SelectList(customersList, "Id", "FullName");

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Reservation", new { errorResponse = response });
            }

            var strData = await response.Content.ReadAsStringAsync();
            var reservation = JsonConvert.DeserializeObject<Reservation>(strData);
            return View(reservation);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Reservation reservation)
        {
            prepareToken();

            string strData = JsonConvert.SerializeObject(reservation, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });
            StringContent content = new StringContent(strData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PatchAsync(reservationsApiUrl, content).Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Reservation", new { errorResponse = response.ReasonPhrase });
            }

            return RedirectToAction("Index");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            prepareToken();

            //query details
            string url = reservationsApiUrl + $"/{id}" +
                $"?$expand={nameof(Reservation.Room)}($select={nameof(Room.Number)})" +
                $"&$expand={nameof(Reservation.Customer)}($select={nameof(Customer.FullName)},{nameof(Customer.Email)})";

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Reservation", new { errorResponse = response });
            }

            var strData = await response.Content.ReadAsStringAsync();
            var reservation = JsonConvert.DeserializeObject<Reservation>(strData);
            return View(reservation);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {

            prepareToken();
            //get available rooms
            HttpResponseMessage roomsRes = client.GetAsync(roomApiUrl).Result;
            var roomJson = roomsRes.Content.ReadAsStringAsync().Result;
            // Parse odata roomsRes to List
            var rooms = JsonConvert.DeserializeObject<dynamic>(roomJson);
            List<Room> roomList = JsonConvert.DeserializeObject<List<Room>>(rooms.value.ToString());
            ViewData["RoomId"] = new SelectList(roomList, "Id", "Number");

            prepareToken();
            //get available Customer
            HttpResponseMessage custRes = client.GetAsync(customerApiUrl).Result;
            var custJson = custRes.Content.ReadAsStringAsync().Result;
            // Parse odata roomsRes to List
            var customers = JsonConvert.DeserializeObject<dynamic>(custJson);
            List<Customer> customersList = JsonConvert.DeserializeObject<List<Customer>>(customers.value.ToString());
            ViewData["CustomerId"] = new SelectList(customersList, "Id", "FullName");
            return View();
        }


        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reservation reservation)
        {
            prepareToken();

            string strData = JsonConvert.SerializeObject(reservation, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });
            StringContent content = new StringContent(strData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(reservationsApiUrl, content).Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Reservation", new { errorResponse = response.ReasonPhrase });
            }

            return RedirectToAction("Index");
        }



        [HttpGet("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            prepareToken();

            HttpResponseMessage response = await client.GetAsync($"{reservationsApiUrl}/{id}");
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Reservation", new { errorResponse = response });
            }

            string strData = await response.Content.ReadAsStringAsync();
            var reservation = JsonConvert.DeserializeObject<Reservation>(strData);
            return View(reservation);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Reservation reservation)
        {
            prepareToken();

            var url = $"{reservationsApiUrl}/{reservation.Id}";

            HttpResponseMessage response = client.DeleteAsync(url).Result;
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Reservation", new { errorResponse = response });
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
