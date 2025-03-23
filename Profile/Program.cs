using System.Data;
using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Profile.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<ProfileDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddMassTransit(mt =>
{
    mt.AddEntityFrameworkOutbox<ProfileDbContext>(o =>
    {
        o.UsePostgres();
    });

    mt.SetEndpointNameFormatter(new MyEndpointNameFormatter("dev"));
    mt.AddConsumers(typeof(Program).Assembly);

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
});

var app = builder.Build();

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
//     var dbContext = scope.ServiceProvider.GetRequiredService<ProfileDbContext>();
//     logger.LogInformation("Applying migrations...");
//     await dbContext.Database.MigrateAsync();
//     logger.LogInformation("Migrations applied.");
// }

app.Run();
