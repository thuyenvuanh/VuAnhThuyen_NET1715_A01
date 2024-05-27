using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository.Models;
using Repository;

namespace Api_OData.Controllers
{
    public class RoomsController: ODataController
    {
        private readonly IRoomRepository roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(roomRepository.GetAll());
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var room = roomRepository.GetRoomById(key);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Post([FromBody] Room room)
        {
            if (room == null) return BadRequest("Object cannot be null.");


            return Created(roomRepository.Create(room));
        }

        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Patch([FromBody] Room room)
        {
            if (room == null) return BadRequest("Object cannot be null.");
            return Ok(roomRepository.Update(room));
        }


        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var p = roomRepository.GetRoomById(key);
            if (p == null) return NotFound();
            roomRepository.Delete(key);
            return NoContent();
        }
    }
}
