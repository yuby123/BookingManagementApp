using API.Contracts;
using API.Data;
using API.Models;


namespace API.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingManagementDbContext _context;

        public BookingRepository(BookingManagementDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Booking> GetAll()
        {
            return _context.Bookings.ToList();
        }

        public Booking GetByGuid(Guid guid)
        {
            return _context.Bookings.FirstOrDefault(b => b.Guid == guid);
        }

        public Booking Create(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                _context.SaveChanges();
                return booking;
            }
            catch
            {
                return null;
            }
        }

        public bool Update(Booking booking)
        {
            try
            {
                _context.Bookings.Update(booking);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Booking booking)
        {
            try
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
