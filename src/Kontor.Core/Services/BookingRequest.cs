namespace Kontor.Core.Services;

public sealed record BookingRequest(int EmployeeId, int ProjectId, DateOnly Date, TimeOnly Start, TimeOnly End, string? Note);
