using MediatR;

namespace Customers.Contracts;
public record CustomerCreated(
    Guid Id,
    string FullName,
    string Email,
    string GitHubUsername,
    DateTime DateOfBirth) : IIntegrationEvent;