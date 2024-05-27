using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Repository.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Converters;
using System.Text;

namespace UI.Controllers
{
    [Route("Customer")]
    public class CustomerController : Controller
    {
        private readonly HttpClient client;
        private const string customerApi = "https://localhost:7139/odata/Customers";

        public CustomerController()
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
                string url = $"{customerApi}" +
                    $"?$select=Id,FullName,Telephone,Email,Birthday" +
                    $"&$filter=(Status eq false)";

                searchResponse = await client.GetAsync(url);
                searchResponse.EnsureSuccessStatusCode();

                string search = await searchResponse.Content.ReadAsStringAsync();
                dynamic tempSearch = JObject.Parse(search);
                List<Customer> searchItems = ((JArray)tempSearch.value).Select(x => new Customer
                {
                    Id = (int)x["Id"],
                    FullName = (string)x["FullName"],
                    Telephone = (string)x["Telephone"],
                    Email = (string)x["Email"],
                    Birthday = (DateTime)x["Birthday"]
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
        public IActionResult Create(Customer customer)
        {
            prepareToken();

            string jsonCustomer = JsonConvert.SerializeObject(customer, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });
            StringContent content = new StringContent(jsonCustomer, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(customerApi, content).Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Customer", new { errorResponse = response.ReasonPhrase });
            }

            return RedirectToAction("Index");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            prepareToken();
            //query details
            string url = $"{customerApi}/{id}" +
                $"?$select=Id,FullName,Telephone,Email,Birthday" +
                $"&$filter=(Status eq false)";

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Customer", new { errorResponse = response });
            }

            var strData = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(strData);
            return View(customer);
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            prepareToken();
            //query details
            string url = $"{customerApi}/{id}" +
                $"?$select=Id,FullName,Telephone,Email,Birthday" +
                $"&$filter=(Status eq false)";

            prepareToken();

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Customer", new { errorResponse = response });
            }

            var jsonCustomer = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(jsonCustomer);
            return View(customer);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer)
        {
            prepareToken();

            string jsonCustomer = JsonConvert.SerializeObject(customer, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" });
            StringContent content = new StringContent(jsonCustomer, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PatchAsync(customerApi, content).Result;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Customer", new { errorResponse = response.ReasonPhrase });
            }

            return RedirectToAction("Index");
        }

        [HttpGet("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            prepareToken();

            var url = $"{customerApi}/{id}" +
                $"?$select=Id,FullName,Telephone,Email,Birthday" +
                $"&$filter=(Status eq false)";

            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Customer", new { errorResponse = response });
            }

            string jsonCustomer = await response.Content.ReadAsStringAsync();
            var customer = JsonConvert.DeserializeObject<Customer>(jsonCustomer);
            return View(customer);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Customer customer)
        {
            prepareToken();

            var url = $"{customerApi}/{customer.Id}";

            HttpResponseMessage response = client.DeleteAsync(url).Result;
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                return RedirectToAction("Index", "Customer", new { errorResponse = response });
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
