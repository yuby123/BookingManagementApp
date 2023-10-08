using API.Contracts;
using API.DTOs.Roles;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "superAdmin")]
public class RoleController : ControllerBase
{
    // Deklarasi variabel untuk repository role
    private readonly IRoleRepository _roleRepository;

    // Konstruktor dengan parameter dependency injection untuk repository role
    public RoleController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }


    // Metode untuk mengambil semua data role
    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua role dari repository
        var result = _roleRepository.GetAll();
        // Memeriksa jika tidak ada data role
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
        var data = result.Select(x => (RoleDto)x);
        // Mengembalikan data role dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<RoleDto>>(data));
    }

    // Metode untuk mengambil data role berdasarkan GUID
    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        // Mengambil role berdasarkan GUID dari repository
        var result = _roleRepository.GetByGuid(guid);
        // Jika data role tidak ditemukan
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
        // Mengembalikan data role dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<RoleDto>((RoleDto)result));
    }


    [HttpPost]
    public IActionResult Create(CreateRoleDto roleDto) //Data Role baru dibuat dengan menggunakan CreateRoleDto
    {
        try
        {

            // Membuat role baru di repository
            var result = _roleRepository.Create(roleDto);

            // Mengembalikan data role yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<RoleDto>((RoleDto)result));
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
    public IActionResult Update(RoleDto roleDto)
    {
        try
        {
            // Mengambil data role berdasarkan GUID dari DTO
            var entity = _roleRepository.GetByGuid(roleDto.Guid);
            // Jika data role tidak ditemukan
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

            // Mengatur data role yang akan diperbarui dari DTO
            Role toUpdate = roleDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data role di repository
            _roleRepository.Update(toUpdate);

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



    // Metode untuk menghapus data role berdasarkan GUID
    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data role berdasarkan GUID
            var entity = _roleRepository.GetByGuid(guid);
            // Jika data role tidak ditemukan
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

            // Menghapus data role dari repository
            _roleRepository.Delete(entity);

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


