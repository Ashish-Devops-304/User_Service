using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options) { }

        public override int SaveChanges()
        {
            var selectedEntityList = ChangeTracker.Entries()
                                    .Where(x => x.Entity is BaseModel &&
                                    (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in selectedEntityList)
            {
                ((BaseModel)entity.Entity).DateCreated = DateTime.Now;
                ((BaseModel)entity.Entity).DateUpdated = DateTime.Now;
            }

            return base.SaveChanges();
        }

        //Creates table for User, User Secret, Address, Payment

        public DbSet<User> User { get; set; }

        public DbSet<UserSecret> UserSecret { get; set; }

        public DbSet<Address> Address { get; set; }

        public DbSet<Payment> Payment { get; set; }

    }
}
