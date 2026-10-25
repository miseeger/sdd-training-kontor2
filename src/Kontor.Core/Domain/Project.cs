namespace Kontor.Core.Domain;

/// <summary>A maintenance order that time is booked on. Fixed master data in phase 1, later from Dockyard.</summary>
public sealed class Project
{
    public const int KeyMaxLength = 20;
    public const int NameMaxLength = 200;

    private Project()
    {
        Key = string.Empty;
        Name = string.Empty;
    }

    public Project(int id, string key, string name)
    {
        Id = id;
        Key = key;
        Name = name;
    }

    public int Id { get; private set; }

    public string Key { get; private set; }

    public string Name { get; private set; }
}
