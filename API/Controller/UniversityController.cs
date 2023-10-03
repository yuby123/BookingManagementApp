using API.Contracts;
using API.DTOs.Universities;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UniversityController : ControllerBase
{
    private readonly IUniversityRepository _universityRepository;
    //Constructor ini menerima sebuah instance dari IUniversityRepository melalui dependency injection dan menyimpannya di dalam field _universityRepository.
    public UniversityController(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {

        var result = _universityRepository.GetAll(); //Data universitas diambil dari repositori
        if (!result.Any()) //Jika tidak ada data yang ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Data Not Found");
        }

        var data = result.Select(x => (UniversityDto)x); //Data universitas diubah menjadi DTO (Data Transfer Object) dengan expicit operator

        return Ok(data); //Respons "Ok" akan mengembalikan data universitas dalam format JSON.
    }

    [HttpGet("{guid}")] //digunakan untuk mendapatkan data universitas berdasarkan GUID yang diberikan sebagai parameter.
    public IActionResult GetByGuid(Guid guid)//Method ini digunakan untuk mendapatkan data universitas berdasarkan GUID.
    {
        var result = _universityRepository.GetByGuid(guid); //Data universitas diambil dari repositori menggunakan GUID yang diberikan.
        if (result is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }
        return Ok((UniversityDto)result); //Respons "Ok" akan mengembalikan data universitas dalam format explicit operator.
    }

    [HttpPost]
    public IActionResult Create(CreateUniversityDto universityDto) //Data universitas baru dibuat dengan menggunakan CreateUniversityDto
    {
        var result = _universityRepository.Create(universityDto);
        if (result is null) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to create data");
        }

        return Ok((UniversityDto)result); //Respons "Ok" akan mengembalikan data universitas dalam format explicit operator
    }

    [HttpPut]
    public IActionResult Update(UniversityDto universityDto)
    {
        var entity = _universityRepository.GetByGuid(universityDto.Guid);// Data universitas yang akan diperbarui diambil menggunakan method GetById.
        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        University toUpdate = universityDto;//Jika data ditemukan, objek universityDto akan diubah menjadi objek University dengan beberapa perubahan, kemudian dipasskan ke repository untuk pembaruan.
        toUpdate.CreatedDate = entity.CreatedDate;

        var result = _universityRepository.Update(toUpdate);
        if (!result)//jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to update data");
        }

        return Ok("Data Updated");//Jika pembaruan berhasil, akan mengembalikan respons "Ok

    }

    [HttpDelete("{guid}")]//digunakan untuk menghapus data universitas berdasarkan GUID.
    public IActionResult Delete(Guid guid)
    {
        var entity = _universityRepository.GetByGuid(guid);// Data universitas yang akan Delet diambil menggunakan method GetById.
        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        var result = _universityRepository.Delete(entity);
        if (!result) //jika Delet gagal, maka akan mengembalikan respons "BadRequest"..
        {
            return BadRequest("Failed to delete data");
        }

        return Ok("Data Deleted"); //Jika Delet berhasil, akan mengembalikan respons "Ok
    }
}