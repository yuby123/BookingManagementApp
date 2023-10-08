using API.Contracts;
using API.Controller;
using API.DTOs.Employees;
using API.Models;
using API.Repositories;
using API.Utilities.Handler;
using API.Utilities.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeController : ControllerBase
{
    // Deklarasi variabel untuk repository employee
    private readonly IEmployeeRepository _employeeRepository;

    // Konstruktor dengan parameter dependency injection untuk repository employee
    public EmployeeController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    // Metode untuk mengambil semua data employee
    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua employee dari repository
        var result = _employeeRepository.GetAll();
        // Memeriksa jika tidak ada data employee
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
        var data = result.Select(x => (EmployeeDto)x);
        // Mengembalikan data employee dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<EmployeeDto>>(data));
    }

    // Metode untuk mengambil data employee berdasarkan GUID
    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        // Mengambil employee berdasarkan GUID dari repository
        var result = _employeeRepository.GetByGuid(guid);
        // Jika data employee tidak ditemukan
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
        // Mengembalikan data employee dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<EmployeeDto>((EmployeeDto)result));
    }

    // Metode untuk membuat data employee baru
    [HttpPost]
    public IActionResult Create(CreatedEmployeeDto employeeDto)
    {
        try
        {
            // Membuat objek employee dari DTO
            Employee toCreate = employeeDto;
            // Menghasilkan NIK baru untuk employee
            toCreate.Nik = GenerateHandler.Nik(_employeeRepository.GetLastNik());
            // Membuat employee baru di repository
            var result = _employeeRepository.Create(toCreate);

            // Mengembalikan data employee yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<EmployeeDto>((EmployeeDto)result));
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

    // Metode untuk memperbarui data employee
    [HttpPut]
    public IActionResult Update(EmployeeDto employeeDto)
    {
        try
        {
            // Mengambil data employee berdasarkan GUID dari DTO
            var entity = _employeeRepository.GetByGuid(employeeDto.Guid);
            // Jika data employee tidak ditemukan
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

            // Mengatur data employee yang akan diperbarui dari DTO
            Employee toUpdate = employeeDto;
            toUpdate.Nik = entity.Nik;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data employee di repository
            _employeeRepository.Update(toUpdate);

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

    // Metode untuk menghapus data employee berdasarkan GUID
    [HttpDelete("{guid}")]
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data employee berdasarkan GUID
            var entity = _employeeRepository.GetByGuid(guid);
            // Jika data employee tidak ditemukan
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

            // Menghapus data employee dari repository
            _employeeRepository.Delete(entity);

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
