using API.Models;

namespace API.DTOs.Universities
{
    // CreateUniversityDto adalah DTO (Data Transfer Object) untuk mengirim data universitas baru.
    public class CreateUniversityDto
    {
        // Kode universitas.
        public string Code { get; set; }

        // Nama universitas.
        public string Name { get; set; }

        // Operator implicit yang mengkonversi CreateUniversityDto menjadi objek University.
        public static implicit operator University(CreateUniversityDto createUniversityDto)
        {
            // Membuat objek University dari data dalam CreateUniversityDto.
            return new University
            {
                Code = createUniversityDto.Code,
                Name = createUniversityDto.Name,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }
    }
}
