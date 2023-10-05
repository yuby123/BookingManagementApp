using API.Models;
using API.Utilities.Enums;

namespace Server.DTOs.Accounts;

public class RegistrationDto
{
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public GenderLevel Gender { get; set; }
    public DateTime HiringDate { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string major { get; set; }
    public string degree { get; set; }
    public float gpa { get; set; }
    public string UnivercityCode { get; set; }
    public string UnivercityName { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }

    public static implicit operator Account(RegistrationDto accountDto)
    {
        return new Account
        {
            Guid = Guid.NewGuid(),
            Password = accountDto.ConfirmPassword,
            Otp = 0,
            IsUsed = true,
            ExpiredTime = DateTime.Now,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }

    public static implicit operator Employee(RegistrationDto registrationDto)
    {
        return new Employee
        {
            FirstName = registrationDto.FirstName,
            LastName = registrationDto.LastName,
            BirthDate = registrationDto.BirthDate,
            Gender = registrationDto.Gender,
            HiringDate = registrationDto.HiringDate,
            Email = registrationDto.Email,
            PhoneNumber = registrationDto.PhoneNumber,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }
    public static implicit operator Education(RegistrationDto registrationDto)
    {
        return new Education
        {
            Guid = Guid.NewGuid(),
            Major = registrationDto.major,
            Degree = registrationDto.degree,
            Gpa = registrationDto.gpa,
            UniversityGuid = Guid.NewGuid(),
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }
    public static implicit operator University(RegistrationDto registrationDto)
    {
        return new University
        {
            Code = registrationDto.UnivercityCode,
            Name = registrationDto.UnivercityName,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }
}