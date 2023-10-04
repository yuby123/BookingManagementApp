using API.DTOs.Rooms;
using FluentValidation;

namespace API.Utilities.Validations.Rooms;

public class CreateRoomValidator : AbstractValidator<CreateRoomDto>
{
    public CreateRoomValidator()
    {
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