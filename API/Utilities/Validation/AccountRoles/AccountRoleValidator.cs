using API.DTOs.AccountRoles; 
using FluentValidation; 

namespace API.Utilities.Validations.AccountRoles
{
    public class AccountRoleValidator : AbstractValidator<AccountRoleDto>
    {
        public AccountRoleValidator()
        {
            // Aturan validasi untuk properti Guid
            RuleFor(e => e.Guid)
                .NotEmpty(); // Properti Guid harus tidak boleh kosong

            // Aturan validasi untuk properti AccountGuid
            RuleFor(e => e.AccountGuid)
                .NotEmpty(); // Properti AccountGuid harus tidak boleh kosong

            // Aturan validasi untuk properti RoleGuid
            RuleFor(e => e.RoleGuid)
                .NotEmpty(); // Properti RoleGuid harus tidak boleh kosong
        }
    }
}
