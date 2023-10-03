using API.Contracts;
using API.Data;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Repositories;
    public class RoomRepository : GeneralRepository<Room>, IRoomRepository
{
    public RoomRepository(BookingManagementDbContext context) : base(context) { }
}

