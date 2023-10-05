using API.DTOs.Accounts;
using FluentValidation;

namespace Server.Utilities.Validation.Accounts;

public class CreateAccountValidation : AbstractValidator<CreateAccountDto>
{
    public CreateAccountValidation()
    {
        //validasi password dengan kriteria tidak boleh kosong, min 8, max 16, huruf besar, huruf kecil, angka dan simbol 
        RuleFor(a => a.Password)
            .NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
                    .MinimumLength(8)
                    .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$")
                    .WithMessage("Password harus mengandung setidaknya satu huruf besar, satu huruf kecil, satu angka, dan satu karakter khusus.");

        RuleFor(a => a.Otp) //validator untuk properti Otp
                .NotEmpty(); //tidak boleh kosong atau nol 

        RuleFor(a => a.IsUsed) //validator untuk properti IsUsed
            .NotEmpty(); //tidak boleh kosong atau nol 
    }

}