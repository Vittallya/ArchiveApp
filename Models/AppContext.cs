using Microsoft.EntityFrameworkCore;
using Models;
using System;

namespace Models
{
    public class AppContext: DbContext
    {
        public AppContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Protocol> Protocols => Set<Protocol>();
        public DbSet<Nationality> Nationalities => Set<Nationality>();
        public DbSet<Organ> Organs => Set<Organ>();
        public DbSet<People> Peoples => Set<People>();
    }
}
