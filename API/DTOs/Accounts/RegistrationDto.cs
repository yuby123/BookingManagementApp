using API.Models;
using API.Utilities.Enums;

namespace API.DTOs.Accounts
{
    public class RegistrationDto
    // Representasi DTO untuk register yang akan input di beberapa tabel sekaligus 
    {
        //properti register yang akan jadi inputan user

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public GenderLevel Gender { get; set; }
        public DateTime HiringDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Major { get; set; }
        public string Degree { get; set; }
        public float Gpa { get; set; }
        public string UniversityCode { get; set; }
        public string UniversityName { get; set; }
        public int Otp { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }



        // Operator implisit untuk mengubah objek RegisterAccountDto menjadi objek Account
        public static implicit operator Account(RegistrationDto accountDto)
        {
            // Inisiasi objek Account dengan data dari objek CreateAccountDto
            return new()
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
            return new()
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
        public static implicit operator University(RegistrationDto registrationDto)
        {
            return new()
            {
                Code = registrationDto.UniversityCode,
                Name = registrationDto.UniversityName,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
        public static implicit operator Education(RegistrationDto registrationDto)
        {
            return new()
            {
                Guid = Guid.NewGuid(),
                Major = registrationDto.Major,
                Degree = registrationDto.Degree,
                Gpa = registrationDto.Gpa,
                UniversityGuid = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }

    }
}