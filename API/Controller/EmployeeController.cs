using API.Contracts;
using API.Controller;
using API.DTOs.Employees;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    //Constructor ini menerima sebuah instance dari IEmployeeRepository melalui dependency injection dan menyimpannya di dalam field _roleRepository.

    public EmployeeController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _employeeRepository.GetAll();//Data Employee diambil dari repositori
        if (!result.Any()) //Jika tidak ada data yang ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Data Not Found");
        }

        var data = result.Select(x => (EmployeeDto)x); //Data Employee diubah menjadi DTO (Data Transfer Object) dengan expicit operator
        return Ok(result);
    }

    [HttpGet("{guid}")] //digunakan untuk mendapatkan data Employee berdasarkan GUID yang diberikan sebagai parameter.
    public IActionResult GetByGuid(Guid guid) //Method ini digunakan untuk mendapatkan data Employee berdasarkan GUID.
    {
        var result = _employeeRepository.GetByGuid(guid); //Data Employee diambil dari repositori menggunakan GUID yang diberikan.
        if (result is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }
        return Ok((EmployeeDto)result); //Respons "Ok" akan mengembalikan data Employee dalam format explicit operator.
    }

    [HttpPost]
    public IActionResult Create(CreatedEmployeeDto employeeDto) //Data Employee baru dibuat dengan menggunakan CreateEmployeeDto
    {
        var result = _employeeRepository.Create(employeeDto); 
        if (result is null) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to create data");
        }

        return Ok((EmployeeDto)result); //Respons "Ok" akan mengembalikan data Employee dalam format explicit operator
    }

    [HttpPut]
    public IActionResult Update(EmployeeDto employeeDto) 
    {

        var existingEmployee = _employeeRepository.GetByGuid(employeeDto.Guid); // Data Employee yang akan diperbarui diambil menggunakan method GetById.

        if (existingEmployee == null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Employee not found");
        }

        Employee toUpdate = employeeDto; //Jika data ditemukan, objek roleDto akan diubah menjadi objek Employee dengan beberapa perubahan, kemudian dipasskan ke repository untuk pembaruan.
        toUpdate.CreatedDate = existingEmployee.CreatedDate;

        var result = _employeeRepository.Update(toUpdate);
        if (!result) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to update data");
        }

        return Ok("Data Updated"); //Jika pembaruan berhasil, akan mengembalikan respons "Ok
    }


    [HttpDelete("{guid}")] //digunakan untuk menghapus data Employee berdasarkan GUID.
    public IActionResult Delete(Guid guid)
    {
        var entity = _employeeRepository.GetByGuid(guid); // Data Employee yang akan Delet diambil menggunakan method GetById.

        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Employee not found");
        }

        var deleted = _employeeRepository.Delete(entity);

        if (!deleted) //jika Delet gagal, maka akan mengembalikan respons "BadRequest"..
        {
            return BadRequest("Failed to delete employee");
        }

        return Ok("Data Deleted"); //Jika Delet berhasil, akan mengembalikan respons "Ok"
    }
}
