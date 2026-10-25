using Kontor.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kontor.Infrastructure.Configurations;

internal sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Key).IsRequired().HasMaxLength(Project.KeyMaxLength);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(Project.NameMaxLength);
        builder.HasIndex(p => p.Key).IsUnique();

        builder.HasData(
            new Project(1, "NSL-ALB-01", "Antriebswartung Albatros"),
            new Project(2, "KBA-TG3-04", "Schildgenerator-Kalibrierung Tiefgrund III"),
            new Project(3, "HFL-PER-02", "Lebenserhaltung prüfen Perihel"));
    }
}
