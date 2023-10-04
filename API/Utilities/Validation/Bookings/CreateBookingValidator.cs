using API.DTOs.Bookings;
using FluentValidation;


namespace API.Utilities.Validations.Bookings
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingDto>
    {

        public CreateBookingValidator()
        {
            // Aturan validasi untuk properti StartDate
            RuleFor(b => b.StartDate)
                .NotNull(); // Properti StartDate tidak boleh null

            // Aturan validasi untuk properti EndDate
            RuleFor(b => b.EndDate)
                .NotEmpty(); // Properti EndDate tidak boleh kosong

            // Aturan validasi untuk properti Status
            RuleFor(b => b.Status)
                .NotNull() // Properti Status tidak boleh null
                .IsInEnum(); // Properti Status harus merupakan nilai dari enum yang valid

            // Aturan validasi untuk properti Remarks
            RuleFor(b => b.Remarks)
                .NotNull(); // Properti Remarks tidak boleh null

            // Aturan validasi untuk properti RoomGuid
            RuleFor(b => b.RoomGuid)
                .NotNull(); // Properti RoomGuid tidak boleh null

            // Aturan validasi untuk properti EmployeeGuid
            RuleFor(b => b.EmployeeGuid)
                .NotNull(); // Properti EmployeeGuid tidak boleh null
        }
    }
}