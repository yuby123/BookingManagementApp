using API.Contracts;
using API.DTOs.Bookings;
using API.Models;
using API.Repositories;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    // Deklarasi variabel untuk repository booking
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IEmployeeRepository _employeeRepository;


    // Konstruktor dengan parameter dependency injection untuk repository booking
    public BookingController(IBookingRepository bookingRepository, IRoomRepository roomRepository, IEmployeeRepository employeeRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _employeeRepository = employeeRepository;
    }

    // Endpoint untuk mendapatkan semua detail booking
    [HttpGet("AllBookingDetails")]
    [Authorize(Roles = "admin, superAdmin")]
    public IActionResult GetAllBookingDetails()
    {
        // Mengambil semua data booking dari repository
        var allBookings = _bookingRepository.GetAll();

        // Mengambil semua data employee (Employee) yang berelasi dengan booking dari repository
        var allEmployees = _employeeRepository.GetAll();

        // Mengambil semua data room dari repository
        var allRooms = _roomRepository.GetAll();

        // Menggabungkan (join) tabel booking, employee, dan room untuk mendapatkan detail booking
        var bookingDetails = (from b in allBookings
                              join e in allEmployees on b.EmployeeGuid equals e.Guid
                              join r in allRooms on b.RoomGuid equals r.Guid
                              select new BookingDetailsDto
                              {
                                  Guid = b.Guid,
                                  BookedNIK = e.Nik, // NIK employee yang melakukan booking
                                  BookedBy = $"{e.FirstName} {e.LastName}", // Nama lengkap employee yang melakukan booking
                                  RoomName = r.Name, // Nama room yang dipesan
                                  StartDate = b.StartDate, // Tanggal mulai booking
                                  EndDate = b.EndDate, // Tanggal akhir booking
                                  Status = b.Status, // Status dari booking
                                  Remarks = b.Remarks // Catatan terkait booking
                              }).ToList();

        // Jika tidak ada detail booking yang ditemukan, kembalikan respons NotFound
        if (!bookingDetails.Any())
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Tidak ada detail booking yang ditemukan"
            });
        }

        // Mengembalikan daftar detail booking dalam bentuk respons OK
        return Ok(new ResponseOKHandler<IEnumerable<BookingDetailsDto>>(bookingDetails));
    }


    // Endpoint untuk mengambil detail booking berdasarkan GUID yang diberikan
    [HttpGet("details/{guid}", Name = "GetBookingByGuid")]
    [Authorize(Roles = "admin, superAdmin")]
    public IActionResult GetBookingByGuid(Guid guid)
    {
        // Mengambil data booking berdasarkan GUID
        var booking = _bookingRepository.GetByGuid(guid);
        // Mengambil semua data employee
        var allEmployees = _employeeRepository.GetAll();
        // Mengambil semua data room
        var allRooms = _roomRepository.GetAll();

        // Jika booking tidak ditemukan
        if (booking == null)
        {
            // Mengembalikan respons NotFound dengan pesan kesalahan
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Booking dengan GUID yang diberikan tidak ditemukan"
            });
        }

        // Membuat objek detail booking dengan join antara booking, employee, dan room
        var bookingDetail = (from b in new[] { booking }
                             join e in allEmployees on b.EmployeeGuid equals e.Guid
                             join r in allRooms on b.RoomGuid equals r.Guid
                             select new BookingDetailsDto
                             {
                                 Guid = b.Guid,
                                 BookedNIK = e.Nik,
                                 BookedBy = $"{e.FirstName} {e.LastName}",
                                 RoomName = r.Name,
                                 StartDate = b.StartDate,
                                 EndDate = b.EndDate,
                                 Status = b.Status,
                                 Remarks = b.Remarks
                             }).FirstOrDefault();

        // Jika detail booking tidak ditemukan
        if (bookingDetail == null)
        {
            return NotFound(new ResponseErrorHandler
            {
                Code = StatusCodes.Status404NotFound,
                Status = HttpStatusCode.NotFound.ToString(),
                Message = "Detail booking dengan GUID yang diberikan tidak ditemukan"
            });
        }

        // Mengembalikan detail booking dalam bentuk respons OK
        return Ok(new ResponseOKHandler<BookingDetailsDto>(bookingDetail));
    }

    // Endpoint untuk mengetahui durasi pemesanan room
    [HttpGet("BookingLength")]
    public IActionResult GetBookingLength()
    {
        try
        {
            // Mengambil semua data booking dan room
            var bookings = _bookingRepository.GetAll();
            var rooms = _roomRepository.GetAll();

            // Mendefinisikan hari yang tidak dihitung (Sabtu dan Minggu)
            var nonWorkingDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };

            // List untuk menyimpan hasil perhitungan durasi pemesanan
            var roomBookingLengths = new List<RoomBookingLengthDto>();

            // Mengiterasi setiap room
            foreach (var room in rooms)
            {
                // Mengambil semua booking untuk room tertentu
                var roomBookings = bookings.Where(b => b.RoomGuid == room.Guid);

                // Jika ada booking untuk room tersebut
                if (roomBookings.Any())
                {
                    // Menghitung durasi total booking
                    var totalBookingLengthInHours = 0;

                    // Mengiterasi setiap booking
                    foreach (var booking in roomBookings)
                    {
                        var startDate = booking.StartDate;
                        var endDate = booking.EndDate;

                        while (startDate <= endDate)
                        {
                            // Menambahkan durasi jika bukan hari Sabtu atau Minggu
                            if (!nonWorkingDays.Contains(startDate.DayOfWeek))
                            {
                                totalBookingLengthInHours += 1;
                            }
                            startDate = startDate.AddHours(1);
                        }
                    }

                    // Mengkonversi durasi dari jam ke hari
                    var totalBookingLengthInDays = totalBookingLengthInHours / 24;
                    var remainingHours = totalBookingLengthInHours % 24;

                    // Menambahkan hasil perhitungan ke list
                    roomBookingLengths.Add(new RoomBookingLengthDto
                    {
                        RoomGuid = room.Guid,
                        RoomName = room.Name,
                        BookingLength = $"{totalBookingLengthInDays} Hari {remainingHours} Jam"
                    });
                }
            }

            // Mengembalikan daftar hasil perhitungan
            return Ok(new ResponseOKHandler<IEnumerable<RoomBookingLengthDto>>(roomBookingLengths));

        }
        // Menangkap pengecualian jika ada kesalahan saat eksekusi kode
        catch (ExceptionHandler ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
            {
                Code = StatusCodes.Status500InternalServerError,
                Status = HttpStatusCode.InternalServerError.ToString(),
                Message = "Failed to calculate booking lengths",
                Error = ex.Message
            });
        }
    }

    // Metode untuk mengambil semua data booking
    [HttpGet]
    public IActionResult GetAll()
    {
        // Mengambil semua booking dari repository
        var result = _bookingRepository.GetAll();
        // Memeriksa jika tidak ada data booking
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
        var data = result.Select(x => (BookingDto)x);
        // Mengembalikan data booking dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<IEnumerable<BookingDto>>(data));
    }

    // Metode untuk mengambil data booking berdasarkan GUID
    [HttpGet("{guid}")]
    public IActionResult GetByGuid(Guid guid)
    {
        // Mengambil booking berdasarkan GUID dari repository
        var result = _bookingRepository.GetByGuid(guid);
        // Jika data booking tidak ditemukan
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
        // Mengembalikan data booking dalam format DTO dengan kode 200
        return Ok(new ResponseOKHandler<BookingDto>((BookingDto)result));
    }


    [HttpPost]
    public IActionResult Create(CreateBookingDto bookingDto) //Data Booking baru dibuat dengan menggunakan CreateBookingDto
    {
        try
        {

            // Membuat booking baru di repository
            var result = _bookingRepository.Create(bookingDto);

            // Mengembalikan data booking yang baru dibuat dalam format DTO dengan kode 200
            return Ok(new ResponseOKHandler<BookingDto>((BookingDto)result));
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
    public IActionResult Update(BookingDto bookingDto)
    {
        try
        {
            // Mengambil data booking berdasarkan GUID dari DTO
            var entity = _bookingRepository.GetByGuid(bookingDto.Guid);
            // Jika data booking tidak ditemukan
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

            // Mengatur data booking yang akan diperbarui dari DTO
            Booking toUpdate = bookingDto;
            toUpdate.CreatedDate = entity.CreatedDate;

            // Memperbarui data booking di repository
            _bookingRepository.Update(toUpdate);

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



    // Metode untuk menghapus data booking berdasarkan GUID
    [HttpDelete("{guid}")]
    [Authorize(Roles = "admin, superAdmin")]
    public IActionResult Delete(Guid guid)
    {
        try
        {
            // Mengambil data booking berdasarkan GUID
            var entity = _bookingRepository.GetByGuid(guid);
            // Jika data booking tidak ditemukan
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

            // Menghapus data booking dari repository
            _bookingRepository.Delete(entity);

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


