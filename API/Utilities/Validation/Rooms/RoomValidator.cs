using API.DTOs.Rooms; // Mengimpor namespace yang diperlukan
using FluentValidation; // Mengimpor pustaka FluentValidation

namespace API.Utilities.Validations.Rooms
{
    public class RoomValidator : AbstractValidator<RoomDto>
    {
        public RoomValidator()
        {
            // Aturan validasi untuk properti Guid
            RuleFor(r => r.Guid)
                .NotEmpty(); // Properti Guid tidak boleh null

            // Aturan validasi untuk properti Name
            RuleFor(r => r.Name)
                .NotEmpty() // Properti Name tidak boleh kosong
                .MaximumLength(100); // Panjang maksimum untuk Name adalah 100 karakter

            // Aturan validasi untuk properti Floor
            RuleFor(r => r.Floor)
                .NotNull();// Properti Floor tidak boleh null


            // Aturan validasi untuk properti Capacity
            RuleFor(r => r.Capacity)
                .NotEmpty(); // Properti Capacity tidak boleh kosong
        }
    }
}
