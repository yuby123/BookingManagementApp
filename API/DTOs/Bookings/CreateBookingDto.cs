﻿using API.Models;
using API.Utilities.Enums;

namespace API.DTOs.Bookings;
public class CreateBookingDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public StatusLevel Status { get; set; }
    public string Remarks { get; set; }
    public Guid RoomGuid { get; set; }
    public Guid EmployeeGuid { get; set; }

    public static implicit operator Booking(CreateBookingDto bookingDto)
    {    // Operator implicit yang mengkonversi CreateBookingDto menjadi objek Booking.
        return new Booking
        {
            StartDate = bookingDto.StartDate,
            EndDate = bookingDto.EndDate,
            Status = bookingDto.Status,
            Remarks = bookingDto.Remarks,
            RoomGuid = bookingDto.RoomGuid,
            EmployeeGuid = bookingDto.EmployeeGuid,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }


}