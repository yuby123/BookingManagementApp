using API.Contracts;
using API.Controller;
using API.DTOs.Bookings;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;

namespace API.Controller;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingRepository _bookingRepository;
    //Constructor ini menerima sebuah instance dari IBookingRepository melalui dependency injection dan menyimpannya di dalam field _roleRepository.
    public BookingController(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    [HttpGet]
    public IActionResult GetAll() 
    {
        var result = _bookingRepository.GetAll(); //Data Booking diambil dari repositori
        if (!result.Any())//Jika tidak ada data yang ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Data Not Found");
        }

        var data = result.Select(x => (BookingDto)x); //Data Booking diubah menjadi DTO (Data Transfer Object) dengan expicit operator

        return Ok(data);

    }

    [HttpGet("{guid}")] //digunakan untuk mendapatkan data Booking berdasarkan GUID yang diberikan sebagai parameter.
    public IActionResult GetByGuid(Guid guid) //Method ini digunakan untuk mendapatkan data Booking berdasarkan GUID.
    {
        var result = _bookingRepository.GetByGuid(guid); //Data Booking diambil dari repositori menggunakan GUID yang diberikan.
        if (result is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }
        return Ok((BookingDto)result); //Respons "Ok" akan mengembalikan data Booking dalam format explicit operator.
    }


    [HttpPost]
    public IActionResult Create(CreateBookingDto bookingDto) //Data Booking baru dibuat dengan menggunakan CreateBookingDto
    {
        var result = _bookingRepository.Create(bookingDto); 
        if (result is null) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to create data");
        }

        return Ok((BookingDto)result); //Respons "Ok" akan mengembalikan data Booking dalam format explicit operator
    }


    [HttpPut]
    public IActionResult Update(BookingDto bookingDto)
    {
        var entity = _bookingRepository.GetByGuid(bookingDto.Guid); // Data Booking yang akan diperbarui diambil menggunakan method GetById.
        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        Booking toUpdate = bookingDto; //Jika data ditemukan, objek roleDto akan diubah menjadi objek Booking dengan beberapa perubahan, kemudian dipasskan ke repository untuk pembaruan.
        toUpdate.CreatedDate = entity.CreatedDate;

        var result = _bookingRepository.Update(toUpdate);
        if (!result) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to update data");
        }

        return Ok("Data Updated"); //Jika pembaruan berhasil, akan mengembalikan respons "Ok
    }



    [HttpDelete("{guid}")] //digunakan untuk menghapus data Room berdasarkan GUID.
    public IActionResult Delete(Guid guid)
    {
        var entity = _bookingRepository.GetByGuid(guid);// Data room yang akan Delet diambil menggunakan method GetById.
        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        var result = _bookingRepository.Delete(entity);
        if (!result) //jika Delet gagal, maka akan mengembalikan respons "BadRequest"..
        {
            return BadRequest("Failed to delete data");
        }

        return Ok("Data Deleted"); //Jika Delet berhasil, akan mengembalikan respons "Ok
    }

}

