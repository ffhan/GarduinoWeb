using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Garduino.Data.Migrations
{
    public partial class MajorRevision1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Measure",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Code",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceName",
                table: "Code",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Measure_UserId",
                table: "Measure",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Code_UserId",
                table: "Code",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Code_AspNetUsers_UserId",
                table: "Code",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Measure_AspNetUsers_UserId",
                table: "Measure",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Code_AspNetUsers_UserId",
                table: "Code");

            migrationBuilder.DropForeignKey(
                name: "FK_Measure_AspNetUsers_UserId",
                table: "Measure");

            migrationBuilder.DropIndex(
                name: "IX_Measure_UserId",
                table: "Measure");

            migrationBuilder.DropIndex(
                name: "IX_Code_UserId",
                table: "Code");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Measure",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Code",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceName",
                table: "Code",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
