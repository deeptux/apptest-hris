using Microsoft.EntityFrameworkCore;

namespace Hris.Demo.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ApplicantFileRecord> ApplicantFiles => Set<ApplicantFileRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var e = modelBuilder.Entity<ApplicantFileRecord>();
        e.ToTable("ApplicantFiles");
        e.HasKey(x => x.Id);
        e.Property(x => x.Category).HasMaxLength(32).IsRequired();
        e.Property(x => x.ObjectKey).HasMaxLength(512).IsRequired();
        e.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
        e.Property(x => x.ContentType).HasMaxLength(128).IsRequired();
        e.Property(x => x.StorageProvider).HasMaxLength(32).IsRequired();
        e.HasIndex(x => new { x.ApplicantId, x.Category, x.IsActive });
        e.HasIndex(x => x.ApplicantId);
    }
}
