using API.Contracts;
using API.DTOs.Bookings;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    // Deklarasi variabel untuk repository booking
    private readonly IBookingRepository _bookingRepository;

    // Konstruktor dengan parameter dependency injection untuk repository booking
    public BookingController(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }


    // Metode untuk mengambil semua data booking
    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua booking dari repository
        var result = _bookingRepository.GetAll();
        // Memeriksa jika tidak ada data booking
        if (!result.Any())
        {
            // Mengembalikan respon error dengan kode 404 jika tidak ada data
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data Not Found"
            });
        }
        // Mengkonversi hasil ke DTO
        var data = result.Select(x => (BookingDto)x);
        // Mengembalikan data booking dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<BookingDto>>(data));
    }

    // Metode untuk mengambil data booking berdasarkan GUID
    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        // Mengambil booking berdasarkan GUID dari repository
        var result = _bookingRepository.GetByGuid(guid);
        // Jika data booking tidak ditemukan
        if (result is null)
        {
            // Mengembalikan respon error dengan kode 404
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data Not Found"
            });
        }
        // Mengembalikan data booking dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<BookingDto>((BookingDto)result));
    }


    [HttpPost]
    public IActionResult Create(CreateBookingDto bookingDto) //Data Booking baru dibuat dengan menggunakan CreateBookingDto
    {
        try
        {

            // Membuat booking baru di repository
            var result = _bookingRepository.Create(bookingDto);

            // Mengembalikan data booking yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<BookingDto>((BookingDto)result));
        }
        catch (ExceptionHandler ex)
        {
            // Jika terjadi error saat pembuatan, mengembalikan respon error dengan kode 500
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to create data",
                Error = ex.Message
            });
        }
    }

    [HttpPut]
    public IActionResult Update(BookingDto bookingDto)
    {
        try
        {
            // Mengambil data booking berdasarkan GUID dari DTO
            var entity = _bookingRepository.GetByGuid(bookingDto.Guid);
            // Jika data booking tidak ditemukan
            if (entity is null)
            {
                // Mengembalikan respon error dengan kode 404
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Not Found"
                });
            }

            // Mengatur data booking yang akan diperbarui dari DTO
            Booking toUpdate = bookingDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data booking di repository
            _bookingRepository.Update(toUpdate);

            // Mengembalikan pesan bahwa data telah diperbarui dengan kode 200
            return Ok(new ResponseOKHandler<string>("Data Updated"));
        }
        catch (ExceptionHandler ex)
        {
            // Jika terjadi error saat pembaruan, mengembalikan respon error dengan kode 500
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to update data",
                Error = ex.Message
            });
        }
    }



    // Metode untuk menghapus data booking berdasarkan GUID
    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data booking berdasarkan GUID
            var entity = _bookingRepository.GetByGuid(guid);
            // Jika data booking tidak ditemukan
            if (entity is null)
            {
                // Mengembalikan respon error dengan kode 404
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Not Found"
                });
            }

            // Menghapus data booking dari repository
            _bookingRepository.Delete(entity);

            // Mengembalikan pesan bahwa data telah dihapus dengan kode 200
            return Ok(new ResponseOKHandler<string>("Data Deleted"));
        }
        catch (ExceptionHandler ex)
        {
            // Jika terjadi error saat penghapusan, mengembalikan respon error dengan kode 500
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to create data",
                Error = ex.Message
            });
        }
    }
}


