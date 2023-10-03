using API.Contracts;
using API.DTOs.Accounts;
using API.Handler;
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
    public IActionResult Create(CreateAccountDto accountDto)
    {
        // Meng-hash kata sandi sebelum menyimpannya ke database.
        string hashedPassword = HashingHandler.HashPassword(accountDto.Password);

        // Mengganti kata sandi asli dengan yang di-hash sebelum menyimpannya ke DTO.
        accountDto.Password = hashedPassword;

        // Memanggil metode Create dari _accountRepository dengan parameter DTO yang sudah di-hash.
        var result = _accountRepository.Create(accountDto);

        // Memeriksa apakah penciptaan data berhasil atau gagal.
        if (result is null)
        {
            return BadRequest("Failed to create data");
        }

        // Mengembalikan data yang berhasil dibuat dalam respons OK.
        return Ok((AccountDto)result);
    }

    // HTTP PUT endpoint untuk memperbarui data Account.
    [HttpPut] //menangani request update ke endpoint /Account
              //parameter berupa objek menggunakan format DTO explicit agar crete data disesuaikan dengan format DTO
    public IActionResult Update(AccountDto accountDto)
    {
        //get data by guid dan menggunakan format DTO 
        var entity = _accountRepository.GetByGuid(accountDto.Guid);
        if (entity is null) //cek apakah data berdasarkan guid tersedia 
        {
            //return Not Found jika data tidak ditemukan
            return NotFound("Id Not Found");
        }
        //convert data DTO dari inputan user menjadi objek Account
        Account toUpdate = accountDto;
        //menyimpan createdate yg lama 
        toUpdate.CreatedDate = entity.CreatedDate;
        toUpdate.Password = HashingHandler.HashPassword(accountDto.Password);

        //update Account dalam repository
        var result = _accountRepository.Update(toUpdate);
        if (!result) //cek apakah update data gagal

        {
            // return pesan BadRequest jika gagal update data
            return BadRequest("Failed to update data");
        }
        // return HTTP OK dengan kode status 200 dan return "data updated" untuk sukses update.
        return Ok("Data Updated");

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