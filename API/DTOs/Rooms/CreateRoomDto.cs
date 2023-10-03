using API.Models;

namespace API.DTOs.Rooms;
public class CreateRoomDto
{
    public string Name { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }

    // Operator implicit yang mengkonversi CreateRoomDto menjadi objek Room.
    public static implicit operator Room(CreateRoomDto createRoomDto)
    {
        // Membuat objek Room dari data dalam CreateRoomDto.
        return new Room
        {
            Name = createRoomDto.Name,
            Floor = createRoomDto.Floor,
            Capacity = createRoomDto.Capacity,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
    }
}
