using API.DTOs.Accounts;
using FluentValidation;

namespace API.Utilities.Validations.Accounts
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountDto>
    {
        public CreateAccountValidator()
        {
            // Aturan validasi untuk properti Password
            RuleFor(e => e.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$")
                .WithMessage("Password harus mengandung setidaknya satu huruf besar, satu huruf kecil, satu angka, dan satu karakter khusus.");

            // Aturan validasi untuk properti Otp
            RuleFor(e => e.Otp)
                .NotEmpty(); // Properti Otp harus tidak boleh kosong

            // Aturan validasi untuk properti IsUsed
            RuleFor(e => e.IsUsed)
                .NotEmpty(); // Properti IsUsed harus tidak boleh kosong

            // Aturan validasi untuk properti ExpiredTime
            RuleFor(e => e.ExpiredTime)
                .NotEmpty() // Properti ExpiredTime harus tidak boleh kosong
                .Must(expiredTime => expiredTime > DateTime.Now); // ExpiredTime harus lebih besar dari waktu saat ini (DateTime.Now)
        }
    }
}