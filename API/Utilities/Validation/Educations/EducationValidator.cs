using API.DTOs.Educations; 
using FluentValidation; 

namespace API.Utilities.Validation.Educations
{
    public class EducationValidator : AbstractValidator<EducationDto>
    {
        public EducationValidator()
        {
            // Aturan validasi untuk properti Guid
            RuleFor(e => e.Guid)
               .NotEmpty(); // Properti Guid tidak boleh kosong

            // Aturan validasi untuk properti Major
            RuleFor(e => e.Major)
               .NotEmpty() // Properti Major tidak boleh kosong
               .MaximumLength(100); // Panjang maksimum untuk Major adalah 100 karakter

            // Aturan validasi untuk properti Degree
            RuleFor(e => e.Degree)
               .NotEmpty() // Properti Degree tidak boleh kosong
               .MaximumLength(100); // Panjang maksimum untuk Degree adalah 100 karakter

            // Aturan validasi untuk properti Gpa
            RuleFor(e => e.Gpa)
                .NotNull() // Properti Gpa tidak boleh null
                .InclusiveBetween(0, 4); // Properti Gpa harus berada dalam rentang 0 hingga 4

            // Aturan validasi untuk properti UniversityGuid
            RuleFor(e => e.UniversityGuid)
               .NotEmpty(); // Properti UniversityGuid tidak boleh kosong

            // Aturan validasi untuk properti Guid (sudah ada sebelumnya)
            RuleFor(e => e.Guid)
               .NotEmpty(); // Properti Guid tidak boleh kosong
        }
    }
}
