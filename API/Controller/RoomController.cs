
using global::API.Contracts;
using global::API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _roomRepository.GetAll();
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            return Ok(result);
        }

        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            var result = _roomRepository.GetByGuid(guid);
            if (result is null)
            {
                return NotFound("Id Not Found");
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create(Room room)
        {
            var result = _roomRepository.Create(room);
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            return Ok(result);
        }

        [HttpPut("{guid}")]
        public IActionResult Update(Guid guid, Room room)
        {
            var existingRoom = _roomRepository.GetByGuid(guid);
            if (existingRoom == null)
            {
                return NotFound("Room not found");
            }

            var result = _roomRepository.Update(room);
            if (!result)
            {
                return BadRequest("Failed to update data");
            }

            return Ok(result);
        }

        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            var existingRoom = _roomRepository.GetByGuid(guid);
            if (existingRoom == null)
            {
                return NotFound("Room not found");
            }

            var result = _roomRepository.Delete(existingRoom);
            if (!result)
            {
                return BadRequest("Failed to delete data");
            }

            return Ok(result);
        }
    }
}
