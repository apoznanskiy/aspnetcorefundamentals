using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts;

public class CityInfoContext : DbContext
{
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

    public CityInfoContext(DbContextOptions<CityInfoContext> dbContextOptions)
        : base(dbContextOptions)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new City("New York") { Id = 1 }, new City("Paris") { Id = 2 }, new City("London") { Id = 3 });

        modelBuilder.Entity<PointOfInterest>().HasData(
            new PointOfInterest("Place 1") { Id=1, CityId = 1 }, new PointOfInterest("Place 2") { Id = 2, CityId = 1 }
        );

        base.OnModelCreating(modelBuilder);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlite("connectionstring");
    //    base.OnConfiguring(optionsBuilder);
    //}
}
