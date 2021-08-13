using Microsoft.EntityFrameworkCore;
using Models;
using System;

namespace Models
{
    public class AppContext: DbContext
    {
        public AppContext(DbContextOptions options) : base(options)
        {
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //ChangeTracker.LazyLoadingEnabled = true;
            //Database.EnsureCreated();
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<Protocol> Protocols => Set<Protocol>();
        public DbSet<People> Peoples => Set<People>();
    }
}
