using API.Contracts;
using API.DTOs.Roles;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller;
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;
    //Constructor ini menerima sebuah instance dari IRoleRepository melalui dependency injection dan menyimpannya di dalam field _roleRepository.
    public RoleController(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }
    [HttpGet]
    public IActionResult GetAll() 
    {
        var result = _roleRepository.GetAll(); //Data Role diambil dari repositori
        if (!result.Any()) //Jika tidak ada data yang ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Data Not Found"); 
        }

        var data = result.Select(x => (RoleDto)x);//Data Role diubah menjadi DTO (Data Transfer Object) dengan expicit operator

        return Ok(data);
    }

    [HttpGet("{guid}")] //digunakan untuk mendapatkan data Role berdasarkan GUID yang diberikan sebagai parameter.
    public IActionResult GetByGuid(Guid guid) //Method ini digunakan untuk mendapatkan data Role berdasarkan GUID.
    {
        var result = _roleRepository.GetByGuid(guid);//Data Role diambil dari repositori menggunakan GUID yang diberikan.
        if (result is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }
        return Ok((RoleDto)result); //Respons "Ok" akan mengembalikan data Role dalam format explicit operator.
    }

    [HttpPost]
    public IActionResult Create(CreateRoleDto roleDto) //Data Role baru dibuat dengan menggunakan CreateRoleDto
    {
        var result = _roleRepository.Create(roleDto);
        if (result is null) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to create data");
        }

        return Ok((RoleDto)result); //Respons "Ok" akan mengembalikan data Role dalam format explicit operator
    }

    [HttpPut]
    public IActionResult Update(RoleDto roleDto)
    {
        var entity = _roleRepository.GetByGuid(roleDto.Guid); // Data Role yang akan diperbarui diambil menggunakan method GetById.
        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        Role toUpdate = roleDto; //Jika data ditemukan, objek roleDto akan diubah menjadi objek Role dengan beberapa perubahan, kemudian dipasskan ke repository untuk pembaruan.
        toUpdate.CreatedDate = entity.CreatedDate;

        var result = _roleRepository.Update(toUpdate);
        if (!result) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to update data");
        }

        return Ok("Data Updated"); //Jika pembaruan berhasil, akan mengembalikan respons "Ok

    }

    [HttpDelete("{guid}")] //digunakan untuk menghapus data Role berdasarkan GUID.
    public IActionResult Delete(Guid guid)
    {
        var entity = _roleRepository.GetByGuid(guid); // Data Role yang akan Delet diambil menggunakan method GetById.
        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        var result = _roleRepository.Delete(entity);
        if (!result)//jika Delet gagal, maka akan mengembalikan respons "BadRequest"..
        {
            return BadRequest("Failed to delete data");
        }

        return Ok("Data Deleted"); //Jika Delet berhasil, akan mengembalikan respons "Ok"
    }
}
