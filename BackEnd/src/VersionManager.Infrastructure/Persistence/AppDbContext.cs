using Microsoft.EntityFrameworkCore;
using VersionManager.Domain.Softwares;

namespace VersionManager.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public DbSet<Software> Softwares => Set<Software>();
    public DbSet<SoftwareVersion> SoftwareVersions => Set<SoftwareVersion>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Software>(b =>
        {
            b.ToTable("Softwares");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
             .HasDefaultValueSql("NEWSEQUENTIALID()");

            b.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique(); 

            b.Property(x => x.Description)
                .HasMaxLength(1000);

            b.Property(x => x.CreatedAt)
                .IsRequired();

            b.HasMany(x => x.Versions)
                .WithOne()
                .HasForeignKey(v => v.SoftwareId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Navigation(x => x.Versions)
                .HasField("_versions")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<SoftwareVersion>(b =>
        {
            b.ToTable("SoftwareVersions");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
             .HasDefaultValueSql("NEWSEQUENTIALID()");

            b.Property(x => x.SoftwareId).IsRequired();

            b.Property(x => x.Version)
                .HasMaxLength(50)
                .IsRequired();

            b.HasIndex(x => x.Version).IsUnique();

            b.Property(x => x.ReleaseDate)
                .HasColumnType("date")
                .IsRequired();

            b.Property(x => x.IsDeprecated).IsRequired();

            b.Property(x => x.DeprecatedAt)
                .HasColumnType("datetime2")
                .IsRequired(false);

            b.HasIndex(x => new { x.SoftwareId, x.Version }).IsUnique();
        });
    }
}
