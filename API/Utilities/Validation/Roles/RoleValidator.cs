using API.DTOs.Roles;
using FluentValidation;

namespace API.Utilities.Validations.Roles
{
    public class RoleValidator : AbstractValidator<RoleDto>
    {
        public RoleValidator()
        {
            // Aturan validasi untuk properti Guid
            RuleFor(e => e.Guid)
                .NotEmpty(); // Properti Guid tidak boleh kosong

            // Aturan validasi untuk properti Name
            RuleFor(e => e.Name)
               .NotEmpty() // Properti Name tidak boleh kosong
               .MaximumLength(100); // Panjang maksimum untuk Name adalah 100 karakter
        }
    }
}