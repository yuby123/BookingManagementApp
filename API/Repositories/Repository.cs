/*using API.Contracts;
using API.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly BookingManagementDbContext _context;

        public Repository(BookingManagementDbContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T? GetByGuid(Guid guid)
        {
            return _context.Set<T>().Find(guid);
        }

        public T? Create(T entity)
        {
            try
            {
                _context.Set<T>().Add(entity);
                _context.SaveChanges();
                return entity;
            }
            catch
            {
                return null;
            }
        }

        public bool Update(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
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
*/