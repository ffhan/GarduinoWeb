using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Garduino.Data.Migrations
{
    public partial class UserUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "Entry",
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
                    ApplicationUserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Device_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Measure_DeviceId",
                table: "Entry",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Code_DeviceId",
                table: "Code",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Device_ApplicationUserId",
                table: "Device",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Code_Device_DeviceId",
                table: "Code",
                column: "DeviceId",
                principalTable: "Device",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Measure_Device_DeviceId",
                table: "Entry",
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
                table: "Entry");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropIndex(
                name: "IX_Measure_DeviceId",
                table: "Entry");

            migrationBuilder.DropIndex(
                name: "IX_Code_DeviceId",
                table: "Code");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Entry");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Code");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Entry",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Code",
                nullable: true);
        }
    }
}
