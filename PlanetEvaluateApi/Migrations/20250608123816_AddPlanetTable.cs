using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanetEvaluateApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Planets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Mass = table.Column<double>(type: "float", nullable: true),
                    Radius = table.Column<double>(type: "float", nullable: true),
                    DistanceFromSun = table.Column<double>(type: "float", nullable: true),
                    NumberOfMoons = table.Column<int>(type: "int", nullable: true),
                    HasAtmosphere = table.Column<bool>(type: "bit", nullable: false),
                    OxygenVolume = table.Column<double>(type: "float", nullable: true),
                    WaterVolume = table.Column<double>(type: "float", nullable: true),
                    HardnessOfRock = table.Column<int>(type: "int", nullable: true),
                    ThreateningCreature = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETUTCDATE()"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Planets");
        }
    }
}
