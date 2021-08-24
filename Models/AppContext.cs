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
            modelBuilder.Entity<People>().HasOne(x => x.Natio).WithMany().OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<People>().HasOne(x => x.Education).WithMany().OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<People>().HasOne(x => x.Party).WithMany().OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<People>().HasOne(x => x.FamilyType).WithMany().OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Protocol>().HasOne(x => x.Social).WithMany().OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Protocol>().HasOne(x => x.Organ).WithMany().OnDelete(DeleteBehavior.SetNull);
        }

        public DbSet<Protocol> Protocols => Set<Protocol>();
        public DbSet<People> Peoples => Set<People>();
        public DbSet<Update> Updates => Set<Update>();

    }
}
