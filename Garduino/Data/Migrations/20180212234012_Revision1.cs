using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Garduino.Data.Migrations
{
    public partial class Revision1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_AspNetUsers_ApplicationUserId",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                table: "Entry");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                table: "Code");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Device",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Device_ApplicationUserId",
                table: "Device",
                newName: "IX_Device_UserId");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {   
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Device_User_UserId",
                table: "Device",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_User_UserId",
                table: "Device");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Device",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Device_UserId",
                table: "Device",
                newName: "IX_Device_ApplicationUserId");

            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                table: "Entry",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                table: "Code",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_AspNetUsers_ApplicationUserId",
                table: "Device",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
