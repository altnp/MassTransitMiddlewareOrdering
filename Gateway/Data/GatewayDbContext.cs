using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;

namespace Gateway.Data;

public class GatewayDbContext : DbContext
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var mapper = new NpgsqlSnakeCaseNameTranslator();

        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(mapper.TranslateMemberName(entity.GetTableName()!));

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(mapper.TranslateMemberName(property.Name));
            }
        }
    }
}
