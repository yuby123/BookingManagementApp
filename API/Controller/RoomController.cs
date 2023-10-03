
using API.DTOs.Rooms;
using API.Contracts;
using API.Models;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    //Constructor ini menerima sebuah instance dari IRoomRepository melalui dependency injection dan menyimpannya di dalam field _roomRepository.
    public RoomController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _roomRepository.GetAll();//Data Room diambil dari repositori
        if (!result.Any()) //Jika tidak ada data yang ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Data Not Found");
        }

        var data = result.Select(x => (RoomDto)x); //Data Room diubah menjadi DTO (Data Transfer Object) dengan expicit operator
        return Ok(data);

    }

    [HttpGet("{guid}")] //digunakan untuk mendapatkan data Room berdasarkan GUID yang diberikan sebagai parameter.
    public IActionResult GetByGuid(Guid guid)//Method ini digunakan untuk mendapatkan data Room berdasarkan GUID.
    {
        var result = _roomRepository.GetByGuid(guid); //Data Room diambil dari repositori menggunakan GUID yang diberikan.
        if (result is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found"); 
        }
        return Ok((RoomDto)result);//Respons "Ok" akan mengembalikan data Room dalam format explicit operator.

    }

    [HttpPost]
    public IActionResult Create(CreateRoomDto roomDto) //Data Room baru dibuat dengan menggunakan CreateRoomDto
    {
        var result = _roomRepository.Create(roomDto);
        if (result is null)//jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to create data");
        }

        return Ok((RoomDto)result); //Respons "Ok" akan mengembalikan data Room dalam format explicit operator
    }

    [HttpPut]
    public IActionResult Update(RoomDto roomDto)
    {
        var entity = _roomRepository.GetByGuid(roomDto.Guid); // Data Room yang akan diperbarui diambil menggunakan method GetById.
        if (entity is null)//Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        Room toUpdate = roomDto; //Jika data ditemukan, objek roomDto akan diubah menjadi objek Room dengan beberapa perubahan, kemudian dipasskan ke repository untuk pembaruan.
        toUpdate.CreatedDate = entity.CreatedDate;

        var result = _roomRepository.Update(toUpdate);
        if (!result) //jika pembuatan gagal, maka akan mengembalikan respons "BadRequest".
        {
            return BadRequest("Failed to update data");
        }

        return Ok("Data Updated"); //Jika pembaruan berhasil, akan mengembalikan respons "Ok
    }



    [HttpDelete("{guid}")] //digunakan untuk menghapus data Room berdasarkan GUID.
    public IActionResult Delete(Guid guid)
    {
        var entity = _roomRepository.GetByGuid(guid);// Data room yang akan Delet diambil menggunakan method GetById.
        if (entity is null) //Jika data tidak ditemukan, maka akan mengembalikan respons "NotFound".
        {
            return NotFound("Id Not Found");
        }

        var result = _roomRepository.Delete(entity);
        if (!result) //jika Delet gagal, maka akan mengembalikan respons "BadRequest"..
        {
            return BadRequest("Failed to delete data");
        }

        return Ok("Data Deleted"); //Jika Delet berhasil, akan mengembalikan respons "Ok"
    }
}
