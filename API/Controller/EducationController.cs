using API.Contracts;
using API.DTOs.Educations;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/[controller]")]
public class EducationController : ControllerBase
{
    // Deklarasi variabel untuk repository education
    private readonly IEducationRepository _educationRepository;

    // Konstruktor dengan parameter dependency injection untuk repository education
    public EducationController(IEducationRepository educationRepository)
    {
        _educationRepository = educationRepository;
    }


    // Metode untuk mengambil semua data education
    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua education dari repository
        var result = _educationRepository.GetAll();
        // Memeriksa jika tidak ada data education
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
        var data = result.Select(x => (EducationDto)x);
        // Mengembalikan data education dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<EducationDto>>(data));
    }

    // Metode untuk mengambil data education berdasarkan GUID
    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        // Mengambil education berdasarkan GUID dari repository
        var result = _educationRepository.GetByGuid(guid);
        // Jika data education tidak ditemukan
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
        // Mengembalikan data education dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<EducationDto>((EducationDto)result));
    }


    [HttpPost]
    public IActionResult Create(CreateEducationDto educationDto) //Data Education baru dibuat dengan menggunakan CreateEducationDto
    {
        try
        {

            // Membuat education baru di repository
            var result = _educationRepository.Create(educationDto);

            // Mengembalikan data education yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<EducationDto>((EducationDto)result));
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
    public IActionResult Update(EducationDto educationDto)
    {
        try
        {
            // Mengambil data education berdasarkan GUID dari DTO
            var entity = _educationRepository.GetByGuid(educationDto.Guid);
            // Jika data education tidak ditemukan
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

            // Mengatur data education yang akan diperbarui dari DTO
            Education toUpdate = educationDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data education di repository
            _educationRepository.Update(toUpdate);

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



    // Metode untuk menghapus data education berdasarkan GUID
    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data education berdasarkan GUID
            var entity = _educationRepository.GetByGuid(guid);
            // Jika data education tidak ditemukan
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

            // Menghapus data education dari repository
            _educationRepository.Delete(entity);

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


