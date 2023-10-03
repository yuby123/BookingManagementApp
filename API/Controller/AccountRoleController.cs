using API.Contracts;
using API.DTOs.AccountRoles;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccountRoleController : ControllerBase
{
    private readonly IAccountRoleRepository _accountRoleRepository;
    //Constructor ini menerima sebuah instance dari IAccountRoleRepository melalui dependency injection dan menyimpannya di dalam field _roleRepository.
    public AccountRoleController(IAccountRoleRepository accountRoleRepository)
    {
        _accountRoleRepository = accountRoleRepository;
    }


    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _accountRoleRepository.GetAll(); //Data AccountRole diambil dari repositori
        if (!result.Any())//Jika tidak ada data yang ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Data Not Found");
        }

        var data = result.Select(x => (AccountRoleDto)x); //Data AccountRole diubah menjadi DTO (Data Transfer Object) dengan expicit operator
        return Ok(result);
    }

    [HttpGet("{guid}")]//digunakan untuk mendapatkan data AccountRole berdasarkan GUID yang diberikan sebagai parameter.
    public IActionResult GetByGuid(Guid guid) //Method ini digunakan untuk mendapatkan data AccountRole berdasarkan GUID.
    {
        var result = _accountRoleRepository.GetByGuid(guid); //Data AccountRole diambil dari repositori menggunakan GUID yang diberikan.
        if (result is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }
        return Ok((AccountRoleDto)result); //Respons "Ok" akan mengembalikan data AccountRole dalam format explicit operator.
    }

    [HttpPost]
    public IActionResult Create(CreateAccountRoleDto accountRoleDto) //Data AccountRole baru dibuat dengan menggunakan CreateAccountRoleDto
    {
        var result = _accountRoleRepository.Create(accountRoleDto);
        if (result is null) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to create data");
        }

        return Ok((AccountRoleDto)result); //Respons "Ok" akan mengembalikan data AccountRole dalam format explicit operator
    }

    [HttpPut]
    public IActionResult Update(AccountRoleDto accountRoleDto)
    {

        var existingEmployee = _accountRoleRepository.GetByGuid(accountRoleDto.Guid); // Data AccountRole yang akan diperbarui diambil menggunakan method GetById.

        if (existingEmployee == null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Employee not found");
        }

        AccountRole toUpdate = accountRoleDto;//Jika data ditemukan, objek roleDto akan diubah menjadi objek AccountRole dengan beberapa perubahan, kemudian dipasskan ke repository untuk pembaruan.
        toUpdate.CreatedDate = existingEmployee.CreatedDate;

        var result = _accountRoleRepository.Update(toUpdate);
        if (!result) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to update data");
        }

        return Ok("Data Updated"); //Jika pembaruan berhasil, akan mengembalikan respons "Ok
    }



    [HttpDelete("{guid}")] //digunakan untuk menghapus data AccountRole berdasarkan GUID.
    public IActionResult Delete(Guid guid) // Data AccountRole yang akan Delet diambil menggunakan method GetById.
    {
        var existingAccount = _accountRoleRepository.GetByGuid(guid);

        if (existingAccount is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Account not found");
        }

        var deleted = _accountRoleRepository.Delete(existingAccount);

        if (!deleted) //jika Delet gagal, maka akan mengembalikan respons "BadRequest"..
        {
            return BadRequest("Failed to delete account");
        }

        return Ok("Data Deleted"); //Jika Delet berhasil, akan mengembalikan respons "Ok"
    }
}


