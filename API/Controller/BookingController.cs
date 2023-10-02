using API.Contracts;
using API.Controller;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace API.Controller;

    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _bookingRepository.GetAll();
            if (!result.Any())
            {
                return NotFound("Data Not Found");
            }

            return Ok(result);
        }

        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            var result = _bookingRepository.GetByGuid(guid);
            if (result is null)
            {
                return NotFound("Id Not Found");
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            var result = _bookingRepository.Create(booking);
            if (result is null)
            {
                return BadRequest("Failed to create data");
            }

            return Ok(result);
        }

        [HttpPut("{guid}")]
        public IActionResult Update(Guid guid, Booking booking)
        {
            var existingBooking = _bookingRepository.GetByGuid(guid);
            if (existingBooking == null)
            {
                return NotFound("Booking not found");
            }

            var result = _bookingRepository.Update(booking);
            if (!result)
            {
                return BadRequest("Failed to update data");
            }

            return Ok(result);
        }

        [HttpDelete("{guid}")]
        public IActionResult Delete(Guid guid)
        {
            var existingBooking = _bookingRepository.GetByGuid(guid);
            if (existingBooking == null)
            {
                return NotFound("Booking not found");
            }

            var result = _bookingRepository.Delete(existingBooking);
            if (!result)
            {
                return BadRequest("Failed to delete data");
            }

            return Ok(result);
        }
    }

