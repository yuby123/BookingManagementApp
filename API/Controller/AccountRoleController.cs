using API.Contracts;
using API.DTOs.AccountRoles;
using API.Models;   
using API.Utilities.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "superAdmin")]

public class AccountRoleController : ControllerBase
{
    // Deklarasi variabel untuk repository accountRole
    private readonly IAccountRoleRepository _accountRoleRepository;

    // Konstruktor dengan parameter dependency injection untuk repository accountRole
    public AccountRoleController(IAccountRoleRepository accountRoleRepository)
    {
        _accountRoleRepository = accountRoleRepository;
    }


    // Metode untuk mengambil semua data accountRole
    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua accountRole dari repository
        var result = _accountRoleRepository.GetAll();
        // Memeriksa jika tidak ada data accountRole
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
        var data = result.Select(x => (AccountRoleDto)x);
        // Mengembalikan data accountRole dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<AccountRoleDto>>(data));
    }

    // Metode untuk mengambil data accountRole berdasarkan GUID
    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        // Mengambil accountRole berdasarkan GUID dari repository
        var result = _accountRoleRepository.GetByGuid(guid);
        // Jika data accountRole tidak ditemukan
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
        // Mengembalikan data accountRole dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<AccountRoleDto>((AccountRoleDto)result));
    }


    [HttpPost]
    public IActionResult Create(CreateAccountRoleDto accountRoleDto) //Data AccountRole baru dibuat dengan menggunakan CreateAccountRoleDto
    {
        try
        {
   
            // Membuat accountRole baru di repository
            var result = _accountRoleRepository.Create(accountRoleDto);

            // Mengembalikan data accountRole yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<AccountRoleDto>((AccountRoleDto)result));
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
    public IActionResult Update(AccountRoleDto accountRoleDto)
    {
        try
        {
            // Mengambil data accountRole berdasarkan GUID dari DTO
            var entity = _accountRoleRepository.GetByGuid(accountRoleDto.Guid);
            // Jika data accountRole tidak ditemukan
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

            // Mengatur data accountRole yang akan diperbarui dari DTO
            AccountRole toUpdate = accountRoleDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data accountRole di repository
            _accountRoleRepository.Update(toUpdate);

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



    // Metode untuk menghapus data accountRole berdasarkan GUID
    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data accountRole berdasarkan GUID
            var entity = _accountRoleRepository.GetByGuid(guid);
            // Jika data accountRole tidak ditemukan
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

            // Menghapus data accountRole dari repository
            _accountRoleRepository.Delete(entity);

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


