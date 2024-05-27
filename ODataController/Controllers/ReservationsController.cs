using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository.Models;
using Repository;
using Microsoft.AspNetCore.OData.Formatter;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Api_OData.Controllers
{
    public class ReservationsController : ODataController
    {
        private readonly IReservationRepository reservationRepository;

        public ReservationsController(IReservationRepository reservationRepository)
        {
            this.reservationRepository = reservationRepository;
        }

        [EnableQuery]
        [Authorize]
        public IActionResult Get()
        {
            var returnVals = reservationRepository.GetAll();
            
            var userData = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.UserData);
            if (userData.Value != "0")
            {
                returnVals = returnVals.Where(r => r.CustomerId.ToString() == userData.Value).ToList();
            }
            return Ok(returnVals);
        }

        [EnableQuery]
        [Authorize]
        public IActionResult Get(int key)
        {
            var reservation = reservationRepository.GetReservationById(key);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        [EnableQuery]
        [Authorize]
        public IActionResult Post([FromBody] Reservation reservation)
        {
            if (reservation == null) return BadRequest("Object cannot be null.");


            return Created(reservationRepository.Create(reservation));
        }

        [EnableQuery]
        [Authorize]
        public IActionResult Patch([FromBody] Reservation reservation)
        {
            if (reservation == null) return BadRequest("Object cannot be null.");
            return Ok(reservationRepository.Update(reservation));
        }


        [Authorize]
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var p = reservationRepository.GetReservationById(key);
            if (p == null) return NotFound();
            reservationRepository.Delete(key);
            return NoContent();
        }
    }
}
