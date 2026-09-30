using CountriesCitiesManagement.Application.Exceptions;
using CountriesCitiesManagement.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CountriesCitiesManagement.Infrastructure.Persistence;

public sealed class CountriesCitiesDbContext : DbContext
{
    public CountriesCitiesDbContext(DbContextOptions<CountriesCitiesDbContext> options): base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<City> Cities => Set<City>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Countries");
            entity.HasKey(country => country.Id);

            entity.Property(country => country.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(country => country.Code)
                .HasMaxLength(3)
                .IsFixedLength()
                .IsUnicode(false)
                .IsRequired();

            entity.HasIndex(country => country.Name)
                .IsUnique()
                .HasDatabaseName("UX_Countries_Name");

            entity.HasIndex(country => country.Code)
                .IsUnique()
                .HasDatabaseName("UX_Countries_Code");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("Cities");
            entity.HasKey(city => city.Id);

            entity.Property(city => city.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.HasIndex(city => new { city.CountryId, city.Name })
                .IsUnique()
                .HasDatabaseName("UX_Cities_CountryId_Name");

            entity.HasOne(city => city.Country)
                .WithMany(country => country.Cities)
                .HasForeignKey(city => city.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

}
