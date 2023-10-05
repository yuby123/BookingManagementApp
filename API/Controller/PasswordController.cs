/*using System.Text;
using API.Data;
using API.DTOs.Accounts;
using API.DTOs.ForgotPassword;
using API.Utilities.Handler;
using API.Utilities.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordController : ControllerBase
{
    private readonly BookingManagementDbContext _dbContext;
    private readonly Dictionary<string, (string OTP, DateTime Expiry)> _otpStorage = new Dictionary<string, (string OTP, DateTime Expiry)>();

    public PasswordController(BookingManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var validator = new ForgotPasswordRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var user = await _dbContext.Employees.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return NotFound("Email tidak ditemukan di database");
        }

        var otp = GenerateRandomOTP();
        var otpKey = Guid.NewGuid().ToString(); // Kunci unik untuk penyimpanan sementara
        var otpExpiry = DateTime.UtcNow.AddMinutes(5); // OTP berlaku selama 5 menit

        // Simpan OTP ke dalam penyimpanan sementara
        _otpStorage.Add(otpKey, (otp, otpExpiry));

        return Ok(new { OTP = otp });
    }

    private static string GenerateRandomOTP()
    {
        // Generate OTP berupa 6 angka acak
        var random = new Random();
        var otp = new StringBuilder();

        for (int i = 0; i < 6; i++)
        {
            otp.Append(random.Next(0, 9));
        }

        return otp.ToString();
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

        if (!_otpStorage.TryGetValue(request.Email, out var otpData))
        {
            return BadRequest("OTP tidak valid");
        }

        if (otpData.Expiry < DateTime.UtcNow)
        {
            _otpStorage.Remove(request.Email); // Hapus OTP yang kadaluarsa dari penyimpanan sementara
            return BadRequest("OTP telah kadaluarsa");
        }

        if (otpData.OTP != request.OTP)
        {
            return BadRequest("OTP tidak sesuai");
        }

        _otpStorage.Remove(request.Email); // Hapus OTP setelah digunakan

        // Periksa apakah NewPassword dan ConfirmPassword sesuai
        if (request.NewPassword != request.ConfirmPassword)
        {
            return BadRequest("NewPassword dan ConfirmPassword tidak sesuai");
        }

        // Lakukan perubahan password di sini
        // Pastikan untuk menyimpan password yang baru ke database

        // Contoh sederhana:
        var user = await _dbContext.Employees.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            return NotFound("Email tidak ditemukan di database");
        }

        var entity = await _dbContext.Accounts.FirstOrDefaultAsync(u => u.Password == request.NewPassword);
        // Update password
        entity.Password = HashingHandler.HashPassword(request.NewPassword); // Anda perlu mengimplementasikan metode HashPassword sesuai kebutuhan aplikasi Anda

        // Simpan perubahan ke database
        await _dbContext.SaveChangesAsync();

        return Ok("Password berhasil diubah");
    }


}*/