using API.Models;

namespace API.DTOs.AccountRoles;

public class CreateAccountRoleDto
{
    public Guid AccountGuid { get; set; }
    public Guid RoleGuid { get; set; }

    // Operator konversi eksplisit untuk mengubah objek AccountRole ke AccountRoleDto.
    // Digunakan ketika perlu mentransfer data AccountRole ke klien API.
    public static implicit operator AccountRole(CreateAccountRoleDto dto)
    {
        return new AccountRole
        {
            AccountGuid = dto.AccountGuid,
            RoleGuid = dto.RoleGuid,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }
}
