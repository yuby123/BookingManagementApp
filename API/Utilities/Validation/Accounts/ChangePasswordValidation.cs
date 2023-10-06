using API.DTOs.Accounts;
using FluentValidation;
using API.DTOs.Accounts;

namespace Server.Utilities.Validation.Accounts;

public class ChangePasswordValidation : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordValidation()
    {

        RuleFor(a => a.NewPassword)
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