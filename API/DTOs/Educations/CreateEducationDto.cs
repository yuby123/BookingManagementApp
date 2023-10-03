using API.Models;

namespace API.DTOs.Educations;
public class CreateEducationDto
{
    public Guid Guid { get; set; }
    public string Major { get; set; }
    public string Degree { get; set; }
    public float Gpa { get; set; }
    public Guid UniversityGuid { get; set; }

    public static implicit operator Education(CreateEducationDto createEducationDto)
    { // Operator implicit yang mengkonversi CreateEducationDto menjadi objek Education .
        return new Education
        {
            Guid = createEducationDto.Guid,
            Major = createEducationDto.Major,
            Degree = createEducationDto.Degree,
            Gpa = createEducationDto.Gpa,
            UniversityGuid = createEducationDto.UniversityGuid,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }
}
