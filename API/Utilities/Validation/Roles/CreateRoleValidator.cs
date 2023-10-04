using API.DTOs.Roles;
using FluentValidation;

namespace API.Utilities.Validations.Roles
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleDto>
    {
        public CreateRoleValidator()
        {
            // Aturan validasi untuk properti Name
            RuleFor(e => e.Name)
               .NotEmpty() // Properti Name tidak boleh kosong
               .MaximumLength(100); // Panjang maksimum untuk Name adalah 100 karakter
        }
    }
}