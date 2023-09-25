using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

public class Roles : BaseEntity
{
    [Column("name", TypeName = "nvarchar(100)")]
    public string Name { get; set; }

}