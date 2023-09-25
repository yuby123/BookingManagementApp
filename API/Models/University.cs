using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;


public class University : BaseEntity
{
    [Column("code", TypeName = "nvarchar(50)")]
    public string Code { get; set; }

    [Column("name", TypeName = "nvarchar(100)")]
    public string Name { get; set; }
}