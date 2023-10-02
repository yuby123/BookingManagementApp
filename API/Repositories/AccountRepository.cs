using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories;
    public class AccountRepository : IAccountRepository
    {
        private readonly BookingManagementDbContext _context;

        public AccountRepository(BookingManagementDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public Account GetByGuid(Guid guid)
        {
            return _context.Accounts.FirstOrDefault(account => account.Guid == guid);
        }

        public Account Create(Account account)
        {
            try
            {
                _context.Accounts.Add(account);
                _context.SaveChanges();
                return account;
            }
            catch
            {
                return null;
            }
        }

        public bool Update(Account account)
        {
            try
            {
                _context.Accounts.Update(account);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Account account)
        {
            try
            {
                _context.Accounts.Remove(account);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

