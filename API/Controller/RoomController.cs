using API.Contracts;
using API.DTOs.Rooms;
using API.Models;
using API.Repositories;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    // Deklarasi variabel untuk repository room
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IEmployeeRepository _employeeRepository;


    // Konstruktor dengan parameter dependency injection untuk repository room
    public RoomController(IRoomRepository roomRepository, IBookingRepository bookingRepository, IEmployeeRepository employeeRepository)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _employeeRepository = employeeRepository;
    }


    // Endpoint untuk mendapatkan ruangan yang digunakan pada hari ini
    [HttpGet("RoomOrderedToday")]
    public IActionResult GetRoomUsedToday()
    {
        // Mengambil semua data booking
        var allBooking = _bookingRepository.GetAll();
        // Mengambil semua data ruangan
        var allRoom = _roomRepository.GetAll();
        // Mengambil semua data employee
        var allEmployees = _employeeRepository.GetAll();

        // Mendapatkan tanggal hari ini
        DateTime today = DateTime.Now.Date;

        // Cek jika tidak ada data booking atau ruangan
        if (!(allBooking.Any() && allRoom.Any()))
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Data Booking atau Room Tidak Ditemukan"
            });
        }

        // Mencari ruangan yang digunakan pada hari ini
        var roomsUsedToday = (from b in allBooking
                              join r in allRoom on b.RoomGuid equals r.Guid
                              join e in allEmployees on b.EmployeeGuid equals e.Guid 
                              where b.StartDate.Date <= today && today <= b.EndDate.Date
                              select new RoomUsedDto
                              {
                                  BookingGuid = b.Guid,
                                  Status = b.Status,
                                  RoomName = r.Name,
                                  Floor = r.Floor,
                                  BookedBy = $"{e.FirstName} {e.LastName}" // Nama lengkap employee yang memesan
                              }).ToList();

        // Jika tidak ada ruangan yang digunakan hari ini
        if (!roomsUsedToday.Any())
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Tidak ada ruangan yang digunakan hari ini"
            });
        }

        // Mengembalikan daftar ruangan yang digunakan hari ini dalam bentuk respons OK
        return Ok(new ResponseOKHandler<IEnumerable<RoomUsedDto>>(roomsUsedToday));
    }

    // Endpoint untuk mendapatkan daftar ruangan yang tersedia
    [HttpGet("AvailableRooms")]
    public IActionResult GetAvailableRooms()
    {
        try
        {
            // Mengambil semua data ruangan
            var allRooms = _roomRepository.GetAll();

            // Mengambil semua data booking
            var allBookings = _bookingRepository.GetAll();

            // Mendapatkan tanggal hari ini
            DateTime today = DateTime.Now.Date;

            // Menentukan ruangan mana saja yang sudah dipesan hari ini
            var usedRoomGuids = allBookings
                .Where(b => b.StartDate.Date <= today && today <= b.EndDate.Date)
                .Select(b => b.RoomGuid)
                .Distinct()
                .ToList();

            // Mengambil daftar ruangan yang belum dipesan
            var availableRooms = allRooms
                .Where(r => !usedRoomGuids.Contains(r.Guid))
                .Select(r => new RoomDto
                {
                    Guid = r.Guid,
                    Name = r.Name,
                    Floor = r.Floor,
                    Capacity = r.Capacity
                })
                .ToList();

            // Jika tidak ada ruangan yang tersedia
            if (!availableRooms.Any())
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Tidak ada ruangan yang tersedia hari ini"
                });
            }
            // Mengembalikan daftar ruangan yang tersedia
            return Ok(new ResponseOKHandler<IEnumerable<RoomDto>>(availableRooms));
        }
        // Penanganan jika terjadi kesalahan
        catch (ExceptionHandler ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to retrieve available rooms",
                Error = ex.Message
            });
        }
    }



    // Metode untuk mengambil semua data room
    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua room dari repository
        var result = _roomRepository.GetAll();
        // Memeriksa jika tidak ada data room
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
        var data = result.Select(x => (RoomDto)x);
        // Mengembalikan data room dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<RoomDto>>(data));
    }

    // Metode untuk mengambil data room berdasarkan GUID
    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        // Mengambil room berdasarkan GUID dari repository
        var result = _roomRepository.GetByGuid(guid);
        // Jika data room tidak ditemukan
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
        // Mengembalikan data room dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<RoomDto>((RoomDto)result));
    }


    [HttpPost]
    [Authorize(Roles = "admin, superAdmin")]
    public IActionResult Create(CreateRoomDto roomDto) //Data Room baru dibuat dengan menggunakan CreateRoomDto
    {
        try
        {

            // Membuat room baru di repository
            var result = _roomRepository.Create(roomDto);

            // Mengembalikan data room yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<RoomDto>((RoomDto)result));
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
    [Authorize(Roles = "admin, superAdmin")]
    public IActionResult Update(RoomDto roomDto)
    {
        try
        {
            // Mengambil data room berdasarkan GUID dari DTO
            var entity = _roomRepository.GetByGuid(roomDto.Guid);
            // Jika data room tidak ditemukan
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

            // Mengatur data room yang akan diperbarui dari DTO
            Room toUpdate = roomDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data room di repository
            _roomRepository.Update(toUpdate);

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



    // Metode untuk menghapus data room berdasarkan GUID
    [HttpDelete("{guid}")]
    [Authorize(Roles = "admin, superAdmin")]
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data room berdasarkan GUID
            var entity = _roomRepository.GetByGuid(guid);
            // Jika data room tidak ditemukan
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

            // Menghapus data room dari repository
            _roomRepository.Delete(entity);

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


