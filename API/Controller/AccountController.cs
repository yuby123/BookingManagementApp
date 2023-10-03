using API.Contracts;
using API.DTOs.Accounts;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    //Constructor ini menerima sebuah instance dari IEducationRepository melalui dependency injection dan menyimpannya di dalam field _roleRepository.
    public AccountController(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _accountRepository.GetAll();//Data Education diambil dari repositori
        if (!result.Any())//Jika tidak ada data yang ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Data Not Found");
        }

        var data = result.Select(x => (AccountDto)x); //Data Education diubah menjadi DTO (Data Transfer Object) dengan expicit operator

        return Ok(data);
    }

    [HttpGet("{guid}")] //digunakan untuk mendapatkan data Education berdasarkan GUID yang diberikan sebagai parameter.
    public IActionResult GetByGuid(Guid guid)  //Method ini digunakan untuk mendapatkan data Education berdasarkan GUID.
    {
        var result = _accountRepository.GetByGuid(guid); //Data Education diambil dari repositori menggunakan GUID yang diberikan.
        if (result is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }
        return Ok((AccountDto)result); //Respons "Ok" akan mengembalikan data Education dalam format explicit operator.
    }

    [HttpPost]
    public IActionResult Create(CreateAccountDto accountDto) //Data Education baru dibuat dengan menggunakan CreateEducationDto
    {
        var result = _accountRepository.Create(accountDto);
        if (result is null) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to create data");
        }

        return Ok((AccountDto)result); //Respons "Ok" akan mengembalikan data Education dalam format explicit operator
    }

    [HttpPut]
    public IActionResult Update(AccountDto accountDto)
    {
        var entity = _accountRepository.GetByGuid(accountDto.Guid); // Data Education yang akan diperbarui diambil menggunakan method GetById.
        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        Account toUpdate = accountDto; //Jika data ditemukan, objek roleDto akan diubah menjadi objek Education dengan beberapa perubahan, kemudian dipasskan ke repository untuk pembaruan.
        toUpdate.CreatedDate = entity.CreatedDate;

        var result = _accountRepository.Update(toUpdate);
        if (!result) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to update data");
        }

        return Ok("Data Updated"); //Jika pembaruan berhasil, akan mengembalikan respons "Ok

    }

    [HttpDelete("{guid}")] //digunakan untuk menghapus data Education berdasarkan GUID.
    public IActionResult Delete(Guid guid)
    {
        var entity = _accountRepository.GetByGuid(guid); // Data Education yang akan Delet diambil menggunakan method GetById.
        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        var result = _accountRepository.Delete(entity);
        if (!result) //jika Delet gagal, maka akan mengembalikan respons "BadRequest"..
        {
            return BadRequest("Failed to delete data");
        }

        return Ok("Data Deleted");  //Jika Delet berhasil, akan mengembalikan respons "Ok"
    }
}