using Kontor.Core.Domain;

namespace Kontor.Core.Abstractions;

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default);
}
