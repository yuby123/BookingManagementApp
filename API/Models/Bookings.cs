namespace API.Models;

public class Bookings
{
    public Guid Guid { get; set; }
    public DateTime StartdDate { get; set; }
    public DateTime EnddDate { get; set; }
    public int Status { get; set; }
    public string Remarks { get; set; }
    public Guid Room { get; set; }
    public Guid Employee { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

}