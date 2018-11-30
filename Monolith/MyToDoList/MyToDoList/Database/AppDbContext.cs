using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyToDoList.Models;

namespace MyToDoList.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ToDo>().ToTable("ToDos");
            modelBuilder.Entity<ToDo>().Property(t => t.IsCompleted).HasDefaultValue(false);
        }

        protected void AddTimestamps()
        {
            var newEntries = ChangeTracker.Entries();
            var newBaseModels = (from e in newEntries where e.Entity is BaseEntity select e).ToList();
            foreach (var newEntry in newBaseModels)
            {
                if (newEntry.State == EntityState.Added)
                {
                    BaseEntity baseEntity = (BaseEntity)newEntry.Entity;
                    if(newEntry.State == EntityState.Added)
                    {
                        baseEntity.CreatedAt = DateTime.UtcNow;
                    }
                    baseEntity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<ToDo> ToDos { get; set; }
    } 
}
