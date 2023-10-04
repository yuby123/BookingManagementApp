using API.Contracts;
using API.DTOs.Rooms;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    // Deklarasi variabel untuk repository room
    private readonly IRoomRepository _roomRepository;

    // Konstruktor dengan parameter dependency injection untuk repository room
    public RoomController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }


    // Metode untuk mengambil semua data room
    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua room dari repository
        var result = _roomRepository.GetAll();
        // Memeriksa jika tidak ada data room
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
        var data = result.Select(x => (RoomDto)x);
        // Mengembalikan data room dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<RoomDto>>(data));
    }

    // Metode untuk mengambil data room berdasarkan GUID
    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        // Mengambil room berdasarkan GUID dari repository
        var result = _roomRepository.GetByGuid(guid);
        // Jika data room tidak ditemukan
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
        // Mengembalikan data room dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<RoomDto>((RoomDto)result));
    }


    [HttpPost]
    public IActionResult Create(CreateRoomDto roomDto) //Data Room baru dibuat dengan menggunakan CreateRoomDto
    {
        try
        {

            // Membuat room baru di repository
            var result = _roomRepository.Create(roomDto);

            // Mengembalikan data room yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<RoomDto>((RoomDto)result));
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
    public IActionResult Update(RoomDto roomDto)
    {
        try
        {
            // Mengambil data room berdasarkan GUID dari DTO
            var entity = _roomRepository.GetByGuid(roomDto.Guid);
            // Jika data room tidak ditemukan
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

            // Mengatur data room yang akan diperbarui dari DTO
            Room toUpdate = roomDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data room di repository
            _roomRepository.Update(toUpdate);

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



    // Metode untuk menghapus data room berdasarkan GUID
    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data room berdasarkan GUID
            var entity = _roomRepository.GetByGuid(guid);
            // Jika data room tidak ditemukan
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

            // Menghapus data room dari repository
            _roomRepository.Delete(entity);

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


