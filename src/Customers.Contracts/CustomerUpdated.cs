namespace Customers.Contracts;

public record CustomerUpdated(
    Guid Id,
    string FullName,
    string Email,
    string GitHubUsername,
    DateTime DateOfBirth) : IIntegrationEvent;
