using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GarduinoWebAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Measure",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AirHumidity = table.Column<double>(nullable: false),
                    AirTemperature = table.Column<double>(nullable: false),
                    SoilDescription = table.Column<string>(nullable: false),
                    SoilMoisture = table.Column<int>(nullable: false),
                    SourceId = table.Column<Guid>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measure", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Measure");
        }
    }
}
