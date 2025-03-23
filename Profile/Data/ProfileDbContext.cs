using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql.NameTranslation;
using ProfileEntity = Profile.Data.Entities.Profile;

namespace Profile.Data;

public class ProfileDbContext : DbContext
{
    public DbSet<ProfileEntity> Profiles { get; set; }

    public ProfileDbContext(DbContextOptions<ProfileDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var mapper = new NpgsqlSnakeCaseNameTranslator();

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.ApplyConfiguration(new ProfileConfiguration());

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

public class ProfileConfiguration : IEntityTypeConfiguration<ProfileEntity>
{
    public void Configure(EntityTypeBuilder<ProfileEntity> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).UseIdentityColumn();
        builder.Property(m => m.ProfileId).IsRequired().HasColumnName("profile_id");
        builder.Property(m => m.FirstName).IsRequired().HasMaxLength(255);
        builder.Property(m => m.LastName).IsRequired().HasMaxLength(255);
        builder.Property(m => m.DateOfBirth).IsRequired();
        builder.HasIndex(m => m.Email).IsUnique();
        builder.HasIndex(m => m.ProfileId).IsUnique();
    }
}
