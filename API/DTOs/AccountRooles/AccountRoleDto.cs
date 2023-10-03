
using API.Models;

namespace API.DTOs.AccountRoles;

public class AccountRoleDto
{
    public Guid Guid { get; set; }
    public Guid AccountGuid { get; set; }
    public Guid RoleGuid { get; set; }

    // Konversi Eksplisit (Explicit Conversion):
    // Metode ini akan mengonversi EmployeeDto ke Employee secara eksplisit jika diperlukan.
    public static explicit operator AccountRoleDto(AccountRole dto)
    {
        return new AccountRoleDto
        {
            Guid = dto.Guid,
            AccountGuid = dto.AccountGuid,
            RoleGuid = dto.RoleGuid
        };
    }

    // Konversi Implisit (Implicit Conversion):
    // Metode ini akan mengonversi EmployeeDto ke Employee secara implisit jika diperlukan.
    public static implicit operator AccountRole(AccountRoleDto dto)
    {
        return new AccountRole
        {
            Guid = dto.Guid,
            AccountGuid = dto.AccountGuid,
            RoleGuid = dto.RoleGuid
        };
    }
}
