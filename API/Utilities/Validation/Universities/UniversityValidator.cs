using API.DTOs.Universities;
using FluentValidation; 

namespace API.Utilities.Validation.Universities
{
    public class UniversityValidator : AbstractValidator<UniversityDto>
    {
        public UniversityValidator()
        {
            // Aturan validasi untuk properti Guid
            RuleFor(e => e.Guid)
                .NotEmpty(); // Properti Guid tidak boleh kosong

            // Aturan validasi untuk properti Code
            RuleFor(e => e.Code)
                .NotEmpty() // Properti Code tidak boleh kosong
                .MaximumLength(50); // Panjang maksimum untuk Code adalah 50 karakter

            // Aturan validasi untuk properti Name
            RuleFor(e => e.Name)
                .NotEmpty() // Properti Name tidak boleh kosong
                .MaximumLength(100); // Panjang maksimum untuk Name adalah 100 karakter
        }
    }
}