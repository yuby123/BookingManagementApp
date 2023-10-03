using API.Contracts;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;
public class GeneralRepository<TEntity> : IGeneralRepository<TEntity> where TEntity : class
{
    private readonly BookingManagementDbContext _context;

    // Konstruktor kelas GeneralRepository yang menerima DbContext sebagai argumen.
    protected GeneralRepository(BookingManagementDbContext context)
    {
        _context = context;
    }

    // Mengambil semua entitas TEntity dari database dan mengembalikannya dalam bentuk daftar.
    public IEnumerable<TEntity> GetAll()
    {
        return _context.Set<TEntity>().ToList();
    }

    // Mengambil entitas TEntity berdasarkan GUID yang diberikan.
    // Kemudian membersihkan ChangeTracker untuk menghindari melacak perubahan yang tidak diinginkan.
    public TEntity? GetByGuid(Guid guid)
    {
        var entity = _context.Set<TEntity>().Find(guid);
        _context.ChangeTracker.Clear();
        return entity;
    }

    // Membuat entitas TEntity baru dan menyimpannya dalam database.
    public TEntity? Create(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
            return entity;
        }
        catch
        {
            return null;
        }
    }

    // Memperbarui entitas TEntity yang ada dalam database.
    public bool Update(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Update(entity);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Menghapus entitas TEntity dari database.
    public bool Delete(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}

