
using API.Models;
using System.Security.Principal;

namespace API.DTOs.Accounts;
    public class AccountDto
    {
    public Guid Guid { get; set; }
    public string Password { get; set; }
    public int Otp { get; set; }
    public bool IsUsed { get; set; }
    public DateTime ExpiredTime { get; set; }

    public static explicit operator AccountDto(Account account)
    {        // Operator konversi eksplisit untuk mengubah objek Account ke AccountDto.
        // Digunakan ketika perlu mentransfer data Account ke klien API.
        return new AccountDto
        {
            Guid = account.Guid,
            Password = account.Password,
            Otp = account.Otp,
            IsUsed = account.IsUsed,
            ExpiredTime = account.ExpiredTime

        };
    }

    public static implicit operator Account(AccountDto accountDto)
    { // Operator konversi implisit untuk mengubah objek AccountDto ke Account.
      // Digunakan ketika menerima data AccountDto dari klien API dan mengkonversinya ke model Account.
        return new Account
        {
            Guid = accountDto.Guid,
            Password = accountDto.Password,
            Otp = accountDto.Otp,
            IsUsed = accountDto.IsUsed,
            ExpiredTime = accountDto.ExpiredTime,
            ModifiedDate = DateTime.Now
        };
    }
}


