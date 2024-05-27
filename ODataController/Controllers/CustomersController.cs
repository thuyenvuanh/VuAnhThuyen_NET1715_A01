using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository;
using Repository.Impl;
using Repository.Models;
using System.Security.Claims;

namespace Api_OData.Controllers
{
    public class CustomersController : ODataController
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [EnableQuery]
        [Authorize]
        public IActionResult Get()
        {
            var returnVals = _customerRepository.GetAll();

            var userData = HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.UserData);
            if (userData.Value != "0")
            {
                return Ok(returnVals.Single(c => c.Id.ToString() == userData.Value));
            }
            return Ok(returnVals);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var customer = _customerRepository.GetByCustomerId(key);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [EnableQuery]
        public IActionResult Post([FromBody] Customer customer)
        {
            if (customer == null) return BadRequest("Object cannot be null.");


            return Created(_customerRepository.Create(customer));
        }

        [EnableQuery]
        public IActionResult Patch([FromBody] Customer customer)
        {
            if (customer == null) return BadRequest("Object cannot be null.");
            return Ok(_customerRepository.Update(customer));
        }


        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var p = _customerRepository.GetByCustomerId(key);
            if (p == null) return NotFound();
            _customerRepository.Delete(key);
            return NoContent();
        }
    }
}
