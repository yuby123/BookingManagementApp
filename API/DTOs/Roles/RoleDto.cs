
using API.Models;

namespace API.DTOs.Roles;
public class RoleDto
{
    public Guid Guid { get; set; }
    public string Name { get; set; }

    public static explicit operator RoleDto(Role role)
    {
        // Operator konversi eksplisit untuk mengubah objek Role ke RoleDto.
        // Digunakan ketika perlu mentransfer data Role ke klien API.
        return new RoleDto
        {
            Guid = role.Guid,
            Name = role.Name
        };
    }

    public static implicit operator Role(RoleDto rolesDto)
    {
        // Operator konversi implisit untuk mengubah objek RoleDto ke Role.
        // Digunakan ketika menerima data RoleDto dari klien API dan mengkonversinya ke model Role.
        return new Role
        {
            Guid = rolesDto.Guid,
            Name = rolesDto.Name,
            ModifiedDate = DateTime.Now
        };
    }
}

