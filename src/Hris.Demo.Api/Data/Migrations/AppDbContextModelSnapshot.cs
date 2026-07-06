using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Hris.Demo.Api.Data;

#nullable disable

namespace Hris.Demo.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
public partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "9.0.7");

        modelBuilder.Entity("Hris.Demo.Api.Data.ApplicantFileRecord", b =>
        {
            b.Property<Guid>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("TEXT");

            b.Property<Guid>("ApplicantId")
                .HasColumnType("TEXT");

            b.Property<string>("Category")
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnType("TEXT");

            b.Property<string>("ContentType")
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnType("TEXT");

            b.Property<bool>("IsActive")
                .HasColumnType("INTEGER");

            b.Property<string>("ObjectKey")
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnType("TEXT");

            b.Property<string>("OriginalFileName")
                .IsRequired()
                .HasMaxLength(260)
                .HasColumnType("TEXT");

            b.Property<long>("SizeBytes")
                .HasColumnType("INTEGER");

            b.Property<string>("StorageProvider")
                .IsRequired()
                .HasMaxLength(32)
                .HasColumnType("TEXT");

            b.Property<DateTimeOffset>("UploadedAtUtc")
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.HasIndex("ApplicantId");

            b.HasIndex("ApplicantId", "Category", "IsActive");

            b.ToTable("ApplicantFiles", (string)null);
        });
#pragma warning restore 612, 618
    }
}
