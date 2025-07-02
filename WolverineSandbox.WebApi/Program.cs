using Azure.Messaging.ServiceBus.Administration;
using JasperFx;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.AzureServiceBus;
using Wolverine.EntityFrameworkCore;
using Wolverine.SqlServer;
using WolverineSandbox.Domain.Events;
using WolverineSandbox.Domain.Repositories;
using WolverineSandbox.WebApi.Commands;
using WolverineSandbox.WebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Step 1:
// For now, this is enough to integrate Wolverine into
// your application, but there'll be "many" more
// options later of course.
builder.Host.UseWolverine(opts =>
{
    //// NOTE: Not sure how to use yet. Maybe have to use it with a message broker.
    //// Right here, tell Wolverine to make every handler "sticky"
    //options.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;

    string connectionString = builder.Configuration["AzureServiceBus:ConnectionString"]
        ?? throw new InvalidOperationException("`AzureServiceBus:ConnectionString` is not configured.");

    opts.UseAzureServiceBus(connectionString)
        .AutoProvision();

    // Configure message's destination, such as a topic or queue.
    opts.PublishMessage<OrderCreated>().ToAzureServiceBusTopic("wolverinesandbox-ordercreated-dev")
        .UseDurableOutbox();

    // Configure listening endpoint for the consumer.
    opts.ListenToAzureServiceBusSubscription("wolverinesandbox-orderprocessor-mys-dev"
        , configureSubscriptionRule: rule =>
        {
            // Use double quotes to enclose the property name with special characters.
            rule.Filter = new SqlRuleFilter("\"tenant-id\" = 'MYS'");
        }
        )
        .FromTopic("wolverinesandbox-ordercreated-dev")
        .UseDurableInbox();

    string mainSqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    string mysSqlConnectionString = builder.Configuration.GetConnectionString("MYS")
        ?? throw new InvalidOperationException("Connection string 'MYS' not found.");
    string idnSqlConnectionString = builder.Configuration.GetConnectionString("IDN")
        ?? throw new InvalidOperationException("Connection string 'IDN' not found.");

    // First, you do have to have a "main" PostgreSQL database for messaging persistence
    // that will store information about running nodes, agents, and non-tenanted operations
    opts.PersistMessagesWithSqlServer(mainSqlConnectionString)
        .RegisterStaticTenants(tenants =>
        {
            tenants.Register("MYS", mysSqlConnectionString);
            tenants.Register("IDN", idnSqlConnectionString);
        });

    opts.Services.AddDbContextWithWolverineManagedMultiTenancy<ApplicationDbContext>((builder, connectionString, _) =>
    {
        builder.UseSqlServer(connectionString.Value);
    });

    // Adding EF Core transactional middleware, saga support,
    // and EF Core support for Wolverine storage operations
    opts.UseEntityFrameworkCoreTransactions();
});

// Step 2:
// Some in memory services for our application, the 
// only thing that matters for now is that these are
// systems built by the application's IOC container.
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<IssueRepository>();

var app = builder.Build();

// Step 3:
// An endpoint to create a new issue that delegates to Wolverine as a mediator.
app.MapPost("/issues/create", (CreateIssue body, IMessageBus bus) => bus.InvokeAsync(body));
//// [WIP] An endpoint to assign an issue to an existing user that delegates to Wolverine as a mediator.
//app.MapPost("/issues/assign", (AssignIssue body, IMessageBus bus) => bus.InvokeAsync(body));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

// Opt into using Oakton for command line parsing
// to unlock built in diagnostics and utility tools within
// your Wolverine application.
return await app.RunJasperFxCommands(args);