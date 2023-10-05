using System;
using System.Linq;
using API.DTOs.Auth;
using API.Data; // Sesuaikan dengan namespace yang sesuai
using API.DTOs.Accounts; // Sesuaikan dengan namespace yang sesuai
using API.Utilities.Handler; // Sesuaikan dengan namespace yang sesuai
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BookingManagementDbContext _dbContext;

        public AuthController(BookingManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            // Validasi request
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email dan Password harus diisi.");
            }

            // Temukan pengguna berdasarkan email
            var user = await _dbContext.Employees.FirstOrDefaultAsync(u => u.Email == request.Email);


            
            if (user == null)
            {
                // Jika email tidak ditemukan di database
                return BadRequest("Email salah");
            }
            var bar = await _dbContext.Accounts.FirstOrDefaultAsync(u => u.Password == request.Password);
            // Validasi password
           
            if (!HashingHandler.VerifyPassword(request.Password, bar.Password))
            {
                // Jika password salah
                return BadRequest("Account or Password is invalid");
            }

            // Berhasil login
            // Di sini, Anda dapat menghasilkan token JWT atau sesi login lainnya, sesuai kebutuhan aplikasi Anda.
            // Selain itu, Anda dapat mengembalikan data pengguna lainnya jika diperlukan.

            // Misalnya, Anda dapat menghasilkan token JWT sebagai berikut:
            //var token = JwtHandler.GenerateToken(bar); // Anda perlu mengimplementasikan metode GenerateToken sesuai kebutuhan aplikasi Anda.

            // Kemudian, kirim token sebagai respons
            return Ok(/*new { Token = token }*/);
        }
    }
}
