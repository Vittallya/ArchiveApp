using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;

namespace Models
{
    public class AppContext: DbContext
    {
        public AppContext(DbContextOptions options) : base(options)
        {
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //ChangeTracker.LazyLoadingEnabled = true;
            //Database.EnsureDeleted();
            //Database.EnsureCreated();

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
        }

        public DbSet<Protocol> Protocols => Set<Protocol>();
        public DbSet<People> Peoples => Set<People>();

    }
}
