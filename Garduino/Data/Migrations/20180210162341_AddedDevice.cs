using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Garduino.Data.Migrations
{
    public partial class AddedDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "Measure",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "Code",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measure_DeviceId",
                table: "Measure",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Code_DeviceId",
                table: "Code",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Code_Device_DeviceId",
                table: "Code",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Measure_Device_DeviceId",
                table: "Measure",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Code_Device_DeviceId",
                table: "Code");

            migrationBuilder.DropForeignKey(
                name: "FK_Measure_Device_DeviceId",
                table: "Measure");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropIndex(
                name: "IX_Measure_DeviceId",
                table: "Measure");

            migrationBuilder.DropIndex(
                name: "IX_Code_DeviceId",
                table: "Code");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Measure");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Code");
        }
    }
}
