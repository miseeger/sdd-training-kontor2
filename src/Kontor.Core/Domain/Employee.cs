namespace Kontor.Core.Domain;

/// <summary>A person who books working time. Fixed master data in phase 1.</summary>
public sealed class Employee
{
    public const int NameMaxLength = 100;

    private Employee()
    {
        Name = string.Empty;
    }

    public Employee(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }
}
