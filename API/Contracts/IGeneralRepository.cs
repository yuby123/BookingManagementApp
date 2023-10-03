namespace API.Contracts;


public interface IGeneralRepository<TEntity> where TEntity : class
{
    // Mendapatkan semua entitas dari repositori.
    IEnumerable<TEntity> GetAll();

    // Mendapatkan entitas berdasarkan GUID tertentu.
    TEntity? GetByGuid(Guid guid);

    // Membuat entitas baru di dalam repositori.
    TEntity? Create(TEntity entity);

    // Memperbarui entitas yang ada di dalam repositori.
    bool Update(TEntity entity);

    // Menghapus entitas dari repositori.
    bool Delete(TEntity entity);
}

