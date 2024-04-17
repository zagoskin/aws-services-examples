using Customers.Contracts;
using MediatR;

namespace Customers.Consumer.Integrations;

internal sealed class CustomerCreatedHandler : INotificationHandler<CustomerCreated>
{
    private readonly ILogger<CustomerCreatedHandler> _logger;

    public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(CustomerCreated notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processed CustomerCreated message: {Body}", notification.ToString());
        return Task.CompletedTask;
    }
}

