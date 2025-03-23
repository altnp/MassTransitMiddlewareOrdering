using System.Reflection;
using System.Text.Json;
using Contracts.Profile;
using Gateway.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Utils.Messaging;
using Endpoint = Gateway.Endpoints.Endpoint;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<GatewayDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddMassTransit(mt =>
{
    mt.SetEndpointNameFormatter(new MyEndpointNameFormatter("dev"));

    mt.AddEntityFrameworkOutbox<GatewayDbContext>(opts =>
    {
        opts.UsePostgres();
    });

    mt.UsingRabbitMq(
        (context, cfg) =>
        {
            cfg.Host(
                "localhost",
                "/",
                h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                }
            );

            cfg.MessageTopology.SetEntityNameFormatter(new MyEntityNameFormatter("dev"));
            cfg.ConfigureEndpoints(context);
        }
    );
    EndpointConvention.Map<CreateProfile>(new Uri("queue:mt-dev_profile-consumers-createprofile")); //Needs a better solution...
});

builder.Services.AddScoped<DeferredPublisherOutbox>();
builder.Services.AddSingleton<PublisherOutbox>();
builder.Services.AddHostedService<PublisherOutboxProcessorService>();

var app = builder.Build();
var endpointTypes = Assembly
    .GetExecutingAssembly()
    .GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Endpoint)));

foreach (var type in endpointTypes)
{
    if (Activator.CreateInstance(type) is Endpoint endpoint)
    {
        endpoint.Register(app);
    }
}

app.UseMiddleware<DeferredPublisherOutboxMiddleware>();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation(
    "Bus Topology: {Topology}",
    JsonSerializer.Serialize(
        app.Services.GetRequiredService<IBusControl>().GetProbeResult(),
        new JsonSerializerOptions { WriteIndented = true }
    )
);

// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();
//     logger.LogInformation("Applying migrations...");
//     await dbContext.Database.MigrateAsync();
//     logger.LogInformation("Migrations applied.");
// }

app.Run();
