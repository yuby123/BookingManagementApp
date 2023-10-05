namespace API.DTOs.Accounts;

public class ForgetPasswordDto
{
    public string Email { get; set; }
    public int Otp { get; set; }
    public DateTime ExpiredDate { get; set; }
}