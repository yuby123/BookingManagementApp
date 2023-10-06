using FluentValidation;
using API.DTOs.Accounts;

namespace Server.Utilities.Validation.Accounts;

public class RegisterationValidation : AbstractValidator<RegistrationDto>
{
    public RegisterationValidation()
    {
        //validasi password dengan kriteria tidak boleh kosong, min 8, max 16, huruf besar, huruf kecil, angka dan simbol 
        RuleFor(a => a.Password)
            .NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$")
                    .WithMessage("Password harus mengandung setidaknya satu huruf besar, satu huruf kecil, satu angka, dan satu karakter khusus.");
        RuleFor(a => a.ConfirmPassword)
            .NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$")
                    .WithMessage("Password harus mengandung setidaknya satu huruf besar, satu huruf kecil, satu angka, dan satu karakter khusus.");

    }
}