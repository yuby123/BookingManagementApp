using API.DTOs.Employees;
using FluentValidation;

namespace API.Utilities.Validation.Employees;

public class EmployeeValidator : AbstractValidator<EmployeeDto>
{
    public EmployeeValidator()
    {
        // Aturan validasi untuk properti Guid
        RuleFor(e => e.Guid)
           .NotEmpty(); // Properti Guid tidak boleh kosong

        // Aturan validasi untuk properti FirstName
        RuleFor(e => e.FirstName)
           .NotEmpty(); // Properti FirstName tidak boleh kosong

        // Aturan validasi untuk properti BirthDate
        RuleFor(e => e.BirthDate)
           .NotEmpty(); // Properti BirthDate tidak boleh kosong

        // Aturan validasi untuk properti Gender
        RuleFor(e => e.Gender)
           .NotNull() // Properti Gender tidak boleh null
           .IsInEnum(); // Properti Gender harus merupakan nilai dari enum yang valid

        // Aturan validasi untuk properti HiringDate
        RuleFor(e => e.HiringDate)
           .NotEmpty(); // Properti HiringDate tidak boleh kosong

        // Aturan validasi untuk properti Email
        RuleFor(e => e.Email)
           .NotEmpty() // Properti Email tidak boleh kosong
           .EmailAddress(); // Properti Email harus memiliki format email yang valid

        // Aturan validasi untuk properti PhoneNumber
        RuleFor(e => e.PhoneNumber)
           .NotEmpty() // Properti PhoneNumber tidak boleh kosong
           .MinimumLength(10) // Panjang minimum untuk PhoneNumber adalah 10 karakter
           .MaximumLength(20); // Panjang maksimum untuk PhoneNumber adalah 20 karakter
    }
}