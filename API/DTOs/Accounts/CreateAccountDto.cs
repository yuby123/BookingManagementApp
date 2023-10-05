using API.Models;

namespace API.DTOs.Accounts;
public class CreateAccountDto
{
    public Guid Employee { get; set; }
    public string Password { get; set; }
    public int Otp { get; set; }
    public bool IsUsed { get; set; }
    public DateTime ExpiredTime { get; set; }

    public static implicit operator Account(CreateAccountDto createAccountDto)
    {        // Operator konversi eksplisit untuk mengubah objek Account ke AccountDto.
        // Digunakan ketika perlu mentransfer data Account ke klien API.
        return new Account

        {
            Guid = createAccountDto.Employee,
            Password = createAccountDto.Password,
            Otp = createAccountDto.Otp,
            IsUsed = createAccountDto.IsUsed,
            ExpiredTime = createAccountDto.ExpiredTime,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }
}

