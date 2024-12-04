using Marten;
using MediatR;
using Oakton;
using Weasel.Core;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Marten;
using WolverinePlayground.Web;

var builder = WebApplication.CreateBuilder(args);

// The almost inevitable inclusion of Swashbuckle:)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// For now, this is enough to integrate Wolverine into
// your application, but there'll be *many* more
// options later of course :-)
builder.Host.UseWolverine(opts =>
{
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();

    opts.Policies.AutoApplyTransactions();

    opts.Discovery
        .CustomizeHandlerDiscovery(cfg =>
        {
            cfg.Excludes.Implements(typeof(IWolverineIgnore));
        });

    Console.WriteLine(opts.DescribeHandlerMatch(typeof(MediatrHandler)));
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// This is the absolute, simplest way to integrate Marten into your
// .NET application with Marten's default configuration
builder.Services.AddMarten(options =>
{
    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Marten")!);

    // Specify that we want to use STJ as our serializer
    options.UseSystemTextJsonForSerialization();

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
}).UseLightweightSessions().IntegrateWithWolverine();

var app = builder.Build();

// An endpoint to create a new issue that delegates to Wolverine as a mediator
app.MapPost("/issues/create", (CreateIssue body, IMessageBus bus) => bus.InvokeAsync(body));

// An endpoint to assign an issue to an existing user that delegates to Wolverine as a mediator
app.MapPost("/issues/assign", (AssignIssue body, IMessageBus bus) => bus.InvokeAsync(body));

// This should not work
app.MapPost("/break", (FakeCommand body, IMessageBus bus) => bus.InvokeAsync(body));

// Swashbuckle inclusion
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

// Opt into using Oakton for command line parsing
// to unlock built in diagnostics and utility tools within
// your Wolverine application
return await app.RunOaktonCommands(args);