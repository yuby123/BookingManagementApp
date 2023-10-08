using API.Contracts;
namespace API.DTOs.Universities;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UniversityController : ControllerBase
{
    // Deklarasi variabel untuk repository university
    private readonly IUniversityRepository _universityRepository;

    // Konstruktor dengan parameter dependency injection untuk repository university
    public UniversityController(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository;
    }


    // Metode untuk mengambil semua data university
    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua university dari repository
        var result = _universityRepository.GetAll();
        // Memeriksa jika tidak ada data university
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
        var data = result.Select(x => (UniversityDto)x);
        // Mengembalikan data university dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<UniversityDto>>(data));
    }

    // Metode untuk mengambil data university berdasarkan GUID
    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        // Mengambil university berdasarkan GUID dari repository
        var result = _universityRepository.GetByGuid(guid);
        // Jika data university tidak ditemukan
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
        // Mengembalikan data university dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<UniversityDto>((UniversityDto)result));
    }


    [HttpPost]
    public IActionResult Create(CreateUniversityDto universityDto) //Data University baru dibuat dengan menggunakan CreateUniversityDto
    {
        try
        {

            // Membuat university baru di repository
            var result = _universityRepository.Create(universityDto);

            // Mengembalikan data university yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<UniversityDto>((UniversityDto)result));
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
    public IActionResult Update(UniversityDto universityDto)
    {
        try
        {
            // Mengambil data university berdasarkan GUID dari DTO
            var entity = _universityRepository.GetByGuid(universityDto.Guid);
            // Jika data university tidak ditemukan
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

            // Mengatur data university yang akan diperbarui dari DTO
            University toUpdate = universityDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data university di repository
            _universityRepository.Update(toUpdate);

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



    // Metode untuk menghapus data university berdasarkan GUID
    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data university berdasarkan GUID
            var entity = _universityRepository.GetByGuid(guid);
            // Jika data university tidak ditemukan
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

            // Menghapus data university dari repository
            _universityRepository.Delete(entity);

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


