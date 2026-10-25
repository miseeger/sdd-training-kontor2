using Kontor.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kontor.Infrastructure.Configurations;

internal sealed class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Date).IsRequired();
        builder.Property(t => t.Start).IsRequired();
        builder.Property(t => t.End).IsRequired();
        builder.Property(t => t.Note).HasMaxLength(TimeEntry.NoteMaxLength);
        builder.Ignore(t => t.ActualDuration);

        builder.HasOne<Employee>().WithMany().HasForeignKey(t => t.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Project>().WithMany().HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(t => new { t.EmployeeId, t.Date });
    }
}
