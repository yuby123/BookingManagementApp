using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class BookingManagementDbContext : DbContext
    {
        public BookingManagementDbContext(DbContextOptions<BookingManagementDbContext> options) : base(options) { }

        //add model to migrate
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<University> Universities { get; set; }

        //pembutan method overrid untuk atribut uniq
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().HasIndex(e => new
            {
                e.Nik,
                e.Email,
                e.PhoneNumber
            }).IsUnique();


            modelBuilder.Entity<University>()
                        .HasMany(e => e.Educations)
                        .WithOne(u => u.University)
                        .HasForeignKey(e => e.UniversityGuid);



            modelBuilder.Entity<Education>()
                        .HasOne(e => e.Employee)
                        .WithOne(e => e.Education)
                        .HasForeignKey<Education>(e => e.Guid);

            modelBuilder.Entity<Employee>()
                        .HasOne(a=> a.Account)
                        .WithOne(e=> e.Employee)
                        .HasForeignKey<Account>(e => e.Guid)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                        .HasMany(a => a.AccountRoles)
                        .WithOne(a => a.Account)
                        .HasForeignKey(a => a.AccountGuid);

            modelBuilder.Entity<AccountRole>()
                        .HasOne(r=> r.Role)
                        .WithMany(a=> a.AccountRoles)
                        .HasForeignKey(r=>r.RoleGuid);

            modelBuilder.Entity<Employee>()
                        .HasMany(b => b.Bookings)
                        .WithOne(e => e.Employee)
                        .HasForeignKey(b => b.EmployeeGuid);

            modelBuilder.Entity<Booking>()
                        .HasOne(r => r.Room)
                        .WithMany(b => b.Bookings)
                        .HasForeignKey(r => r.RoomGuid);



        }
    }
}
