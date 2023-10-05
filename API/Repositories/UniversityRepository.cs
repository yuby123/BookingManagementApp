using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories;

public class UniversityRepository : GeneralRepository<University>, IUniversityRepository
{
    private readonly BookingManagementDbContext _context;
    public UniversityRepository(BookingManagementDbContext context) : base(context) { }
    public University GetByCodeAndName(string code, string name)
    {
        var entity = _context.Set<University>().FirstOrDefault(e => e.Name == name && e.Code == code);
        _context.ChangeTracker.Clear();
        return entity;
    }
}