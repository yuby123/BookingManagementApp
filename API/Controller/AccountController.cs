using API.Contracts;
using API.DTOs.Accounts;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    //Constructor ini menerima sebuah  instance dari IAccountRepository melalui dependency injection dan menyimpannya di dalam field _accountRepository.
    public AccountController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua account dari repository
        var result = _accountRepository.GetAll();
        // Memeriksa jika tidak ada data account
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
        var data = result.Select(x => (AccountDto)x);
        // Mengembalikan data account dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<AccountDto>>(data));
    }

    [HttpGet("{guid}")] //digunakan untuk mendapatkan data Account berdasarkan GUID yang diberikan sebagai parameter.
    public IActionResult GetByGuid(Guid guid)  //Method ini digunakan untuk mendapatkan data Account berdasarkan GUID.
    {
        // Mengambil account berdasarkan GUID dari repository
        var result = _accountRepository.GetByGuid(guid);
        // Jika data account tidak ditemukan
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
        // Mengembalikan data account dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<AccountDto>((AccountDto)result));
    }

    [HttpPost]
    public IActionResult Create(CreateAccountDto createAccountDto)
    {
        try
        {
            //Hashing
            Account toCreate = createAccountDto;
            toCreate.Password = HashingHandler.HashPassword(toCreate.Password);

            //Mapping secara implisit pada createAccountDto untuk dijadikan objek Account
            var result = _accountRepository.Create(toCreate);


            // Mengembalikan data account yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<AccountDto>((AccountDto)result));
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

    // HTTP PUT endpoint untuk memperbarui data Account.
    [HttpPut] //menangani request update ke endpoint /Account
              //parameter berupa objek menggunakan format DTO explicit agar crete data disesuaikan dengan format DTO
    public IActionResult Update(AccountDto accountDto)
    {
     
        try
        {
            //get data by guid dan menggunakan format DTO 
            var entity = _accountRepository.GetByGuid(accountDto.Guid);
            // Jika data account tidak ditemukan
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

            //convert data DTO dari inputan user menjadi objek Account
            Account toUpdate = accountDto;
            //menyimpan createdate yg lama 
            toUpdate.CreatedDate = entity.CreatedDate;
            toUpdate.Password = HashingHandler.HashPassword(accountDto.Password);

            // Memperbarui data account di repository
            _accountRepository.Update(toUpdate);

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

    [HttpDelete("{guid}")] //digunakan untuk menghapus data Account berdasarkan GUID.
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data account berdasarkan GUID
            var entity = _accountRepository.GetByGuid(guid);
            // Jika data account tidak ditemukan
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

            // Menghapus data account dari repository
            _accountRepository.Delete(entity);

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