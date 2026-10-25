using Kontor.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kontor.Infrastructure.Configurations;

internal sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(Employee.NameMaxLength);

        builder.HasData(
            new Employee(1, "Ilka Brandt"),
            new Employee(2, "Tomás Okoye"),
            new Employee(3, "Juna Sato"),
            new Employee(4, "Rik Halvorsen"));
    }
}
