using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Profile.Migrations
{
    /// <inheritdoc />
    public partial class ProfileEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "profiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    last_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profiles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_profiles_email",
                table: "profiles",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_profiles_profile_id",
                table: "profiles",
                column: "profile_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "profiles");
        }
    }
}
