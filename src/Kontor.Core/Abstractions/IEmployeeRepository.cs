using Kontor.Core.Domain;

namespace Kontor.Core.Abstractions;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
}
