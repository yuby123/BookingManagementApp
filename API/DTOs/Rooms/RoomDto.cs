using API.Models;
// DTO digunakan untuk mentransfer data antara klien dan server dalam API.
namespace API.DTOs.Rooms;
public class RoomDto
{
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }

    public static explicit operator RoomDto(Room room)
    {
        // Operator konversi eksplisit untuk mengubah objek Room ke RoomDto.
        // Digunakan ketika perlu mentransfer data Room ke klien API.
        return new RoomDto
        {
            Guid = room.Guid,
            Name = room.Name,
            Floor = room.Floor,
            Capacity = room.Capacity
        };
    }

    //update data
    public static implicit operator Room(RoomDto roomDto)
    {
        // Operator konversi implisit untuk mengubah objek RoomDto ke Room.
        // Digunakan ketika menerima data RoomDto dari klien API dan mengkonversinya ke model Room.
        return new Room
        {
            Guid = roomDto.Guid,
            Name = roomDto.Name,
            Floor = roomDto.Floor,
            Capacity = roomDto.Capacity,
            ModifiedDate = DateTime.Now // Mengatur ModifiedDate dengan waktu saat ini
        };
    }
}