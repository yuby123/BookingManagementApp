
using API.Models;

namespace API.DTOs.Roles;
public class CreateRoleDto
{
    public string Name { get; set; }
    // Operator implicit yang mengkonversi CreateRoleDto menjadi objek Role.
    public static implicit operator Role(CreateRoleDto createRolesDto)
    {
        return new Role
        {
            Name = createRolesDto.Name,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }
}

