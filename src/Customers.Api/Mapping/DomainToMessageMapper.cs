using Customers.Api.Domain;
using Customers.Contracts;

namespace Customers.Api.Mapping;

internal static class DomainToMessageMapper
{
    internal static CustomerCreated ToCustomerCreatedMessage(this Customer customer)
    {
        return new CustomerCreated(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.GitHubUsername,
            customer.DateOfBirth);
    }

    internal static CustomerUpdated ToCustomerUpdatedMessage(this Customer customer)
    {
        return new CustomerUpdated(
            customer.Id,
            customer.FullName,
            customer.Email,
            customer.GitHubUsername,
            customer.DateOfBirth);
    }

    internal static CustomerDeleted ToCustomerDeletedMessage(this Customer customer)
    {
        return new CustomerDeleted(customer.Id);
    }
}
