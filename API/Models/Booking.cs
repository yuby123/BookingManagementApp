using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;


[Table("tb_tr_bookings")]
public class Booking : BaseEntity
{
    [Column("start_date")]
    public DateTime StartdDate { get; set; }

    [Column("end_date")]
    public DateTime EnddDate { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [Column("remarks", TypeName = "nvarchar(max)")]
     public string Remarks { get; set; }

    [Column("room_guid")]
    public Guid Room { get; set; }

    [Column("employee_guid")]
    public Guid Employee { get; set; }

}