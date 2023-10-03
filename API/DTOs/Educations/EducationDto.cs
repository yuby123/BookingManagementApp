using API.Models;

namespace API.DTOs.Educations;
public class EducationDto
{
    public Guid Guid { get; set; }
    public string Major { get; set; }
    public string Degree { get; set; }
    public float Gpa { get; set; }
    public Guid UniversityGuid { get; set; }

    public static explicit operator EducationDto(Education education)
    {
        // Operator konversi eksplisit untuk mengubah objek Education ke EducationDto.
        // Digunakan ketika perlu mentransfer data Education ke klien API.
        return new EducationDto
        {
            Guid = education.Guid,
            Major = education.Major,
            Degree = education.Degree,
            Gpa = education.Gpa,
            UniversityGuid = education.UniversityGuid
        };
    }

    public static implicit operator Education(EducationDto educationDto)
    {
        // Operator konversi implisit untuk mengubah objek EducationDto ke Education.
        // Digunakan ketika menerima data EducationDto dari klien API dan mengkonversinya ke model Education.
        return new Education
        {
            Guid = educationDto.Guid,
            Major = educationDto.Major,
            Degree = educationDto.Degree,
            Gpa = educationDto.Gpa,
            UniversityGuid = educationDto.UniversityGuid,
            ModifiedDate = DateTime.Now
        };
    }
}

