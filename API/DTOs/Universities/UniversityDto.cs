using API.Models;

// DTO digunakan untuk mentransfer data antara klien dan server dalam API.
namespace API.DTOs.Universities;

public class UniversityDto
{
    public Guid Guid { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }

    public static explicit operator UniversityDto(University university)
    {
        // Operator konversi eksplisit untuk mengubah objek University ke UniversityDto.
        // Digunakan ketika perlu mentransfer data University ke klien API.
        return new UniversityDto
        {
            Guid = university.Guid,
            Code = university.Code,
            Name = university.Name
        };
    }

    //update data
    public static implicit operator University(UniversityDto universityDto)
    {
         // Operator konversi implisit untuk mengubah objek UniversityDto ke University.
         // Digunakan ketika menerima data UniversityDto dari klien API dan mengkonversinya ke model University.
        return new University
        {
            Guid = universityDto.Guid,
            Code = universityDto.Code,
            Name = universityDto.Name,
            ModifiedDate = DateTime.Now // Mengatur ModifiedDate dengan waktu saat ini
        };
    }
}
