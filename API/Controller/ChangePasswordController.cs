/*using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Accounts;
using API.Utilities.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChangePasswordController : ControllerBase
    {
        private readonly BookingManagementDbContext _dbContext;
        private readonly Dictionary<string, string> _otpStorage = new Dictionary<string, string>();

        public ChangePasswordController(BookingManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var validator = new ChangePasswordRequestValidator();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            // Buat OTP secara otomatis (misalnya, 6 digit acak)
            var otp = GenerateRandomOTP();

            // Simpan OTP dalam penyimpanan sementara
            _otpStorage[request.Email] = otp;

            // Ganti password untuk pengguna
            var user = await _dbContext.Employees.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound("Email tidak ditemukan di database");
            }

            // Simpan password baru di sini (sesuai dengan implementasi Anda)
            // Misalnya, Anda dapat menggunakan Identity Framework:
            // user.PasswordHash = HashPassword(request.NewPassword);

            return Ok("Password berhasil diubah.");
        }

        // Metode untuk menghasilkan OTP secara otomatis
        private string GenerateRandomOTP()
        {
            Random random = new Random();
            int otpValue = random.Next(100000, 999999);
            return otpValue.ToString("D6"); // Format menjadi 6 digit dengan leading zero jika diperlukan
        }
    }
}
*/