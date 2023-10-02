using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly BookingManagementDbContext _context;

        public RoleRepository(BookingManagementDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Role> GetAll()
        {
            return _context.Roles.ToList();
        }

        public Role? GetByGuid(Guid guid)
        {
            return _context.Roles.Find(guid);
        }

        public Role? Create(Role role)
        {
            try
            {
                _context.Roles.Add(role);
                _context.SaveChanges();
                return role;
            }
            catch
            {
                return null;
            }
        }

        public bool Update(Role role)
        {
            try
            {
                _context.Roles.Update(role);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Role role)
        {
            try
            {
                _context.Roles.Remove(role);
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
