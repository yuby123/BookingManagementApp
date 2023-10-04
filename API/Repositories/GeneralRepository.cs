using API.Contracts;
using API.Data;
using API.Utilities.Handler;
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
        catch (Exception ex)
        {
            if (ex.InnerException is not null && ex.InnerException.Message.Contains("IX_tb_m_employees_nik"))
            {
                throw new ExceptionHandler("NIK already exists");
            }
            if (ex.InnerException is not null && ex.InnerException.Message.Contains("IX_tb_m_employees_email"))
            {
                throw new ExceptionHandler("Email already exists");
            }
            if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_tb_m_employees_phone_number"))
            {
                throw new ExceptionHandler("Phone number already exists");
            }
            throw new ExceptionHandler(ex.InnerException?.Message ?? ex.Message);
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
        catch (Exception ex)
        {
            throw new ExceptionHandler(ex.InnerException?.Message ?? ex.Message);
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
        catch (Exception ex)
        {
            throw new ExceptionHandler(ex.InnerException?.Message ?? ex.Message);
        }
    }
}

