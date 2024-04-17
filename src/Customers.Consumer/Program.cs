
using Amazon.SQS;
using Customers.Consumer;

var builder = WebApplication.CreateSlimBuilder(args);
{
    builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(QueueSettings.SectionName));
    builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
    builder.Services.AddHostedService<QueueConsumerService>();
    builder.Services.AddMediatR(x =>
    {
        x.RegisterServicesFromAssemblyContaining<Program>();
    });
}

var app = builder.Build();
{

    app.MapGet("/ping", () => "pong");

    app.Run();

}
