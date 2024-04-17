using Customers.Contracts;
using MediatR;

namespace Customers.Consumer.Integrations;

internal sealed class CustomerDeletedHandler : INotificationHandler<CustomerDeleted>
{
    private readonly ILogger<CustomerDeletedHandler> _logger;

    public CustomerDeletedHandler(ILogger<CustomerDeletedHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(CustomerDeleted notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processed CustomerDeleted message: {Body}", notification.ToString());
        return Task.CompletedTask;
    }
}

