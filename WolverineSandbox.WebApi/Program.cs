using Oakton;
using Wolverine;
using WolverineSandbox.Domain.Repositories;
using WolverineSandbox.WebApi.Commands;

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
builder.Host.UseWolverine();

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

// Opt into using Oakton for command line parsing
// to unlock built in diagnostics and utility tools within
// your Wolverine application.
return await app.RunOaktonCommands(args);