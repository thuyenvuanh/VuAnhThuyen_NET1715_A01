using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository;
using Repository.Impl;
using Repository.Models;

namespace Api_OData.Controllers
{
    public class RoomTypesController: ODataController
    {
        private readonly IRoomTypeRepository roomTypeRepository;

        public RoomTypesController(IRoomTypeRepository roomTypeRepository)
        {
            this.roomTypeRepository = roomTypeRepository;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(roomTypeRepository.GetAll());
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            var roomType = roomTypeRepository.GetRoomTypeById(key);
            if (roomType == null) return NotFound();
            return Ok(roomType);
        }

        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Post([FromBody] RoomType roomType)
        {
            if (roomType == null) return BadRequest("Object cannot be null.");


            return Created(roomTypeRepository.Create(roomType));
        }

        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Patch([FromBody] RoomType roomType)
        {
            if (roomType == null) return BadRequest("Object cannot be null.");
            return Ok(roomTypeRepository.Update(roomType));
        }


        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var p = roomTypeRepository.GetRoomTypeById(key);
            if (p == null) return NotFound();
            roomTypeRepository.Delete(key);
            return NoContent();
        }
    }
}
