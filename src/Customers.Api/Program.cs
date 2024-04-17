using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Customers.Api.Messaging;
using Customers.Api.Repositories;
using Customers.Api.Services;
using Customers.Api.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

var config = builder.Configuration;
config.AddEnvironmentVariables("CustomersApi_");

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation(x =>
{   
    x.DisableDataAnnotationsValidation = true;
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//SqlMapper.AddTypeHandler(new GuidTypeHandler());
//SqlMapper.RemoveTypeMap(typeof(Guid));
//SqlMapper.RemoveTypeMap(typeof(Guid?));

//builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
//    new SqliteConnectionFactory(config.GetValue<string>("Database:ConnectionString")!));
//builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();

// amazon
builder.Services.Configure<TopicSettings>(config.GetSection(TopicSettings.SectionName));
builder.Services.AddSingleton<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();
builder.Services.AddSingleton<ISnsMessenger, SnsMessenger>();
builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();


builder.Services.AddSingleton<ICustomerRepository, DynamoDBCustomerRepository>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IGitHubService, GitHubService>();
builder.Services.AddSingleton<ICustomerImageService, S3CustomerImageService>();

builder.Services.AddHttpClient("GitHub", httpClient =>
{
    httpClient.BaseAddress = new Uri(config.GetValue<string>("GitHub:ApiBaseUrl")!);
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Accept, "application/vnd.github.v3+json");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.UserAgent, $"Course-{Environment.MachineName}");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.MapControllers();

//var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
//await databaseInitializer.InitializeAsync();

app.Run();
