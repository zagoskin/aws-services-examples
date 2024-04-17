using Customers.Contracts;
using MediatR;

namespace Customers.Consumer.Integrations;

internal sealed class CustomerUpdatedHandler : INotificationHandler<CustomerUpdated>
{
    private readonly ILogger<CustomerUpdatedHandler> _logger;

    public CustomerUpdatedHandler(ILogger<CustomerUpdatedHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(CustomerUpdated notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processed CustomerUpdated message: {Body}", notification.ToString());
        return Task.CompletedTask;
    }
}

