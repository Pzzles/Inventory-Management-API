using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RepairsAndAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Repairs");

            migrationBuilder.RenameColumn(
                name: "RepairDateUtc",
                table: "Repairs",
                newName: "LoggedAtUtc");

            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "Repairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "Repairs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "Repairs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Repairs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Repairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Vendor",
                table: "Repairs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "OccurredAtUtc",
                table: "AuditLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OperatorName",
                table: "AuditLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "AuditLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_AssetId",
                table: "Repairs",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repairs_Assets_AssetId",
                table: "Repairs",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repairs_Assets_AssetId",
                table: "Repairs");

            migrationBuilder.DropIndex(
                name: "IX_Repairs_AssetId",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "Vendor",
                table: "Repairs");

            migrationBuilder.DropColumn(
                name: "OccurredAtUtc",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "OperatorName",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "LoggedAtUtc",
                table: "Repairs",
                newName: "RepairDateUtc");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Repairs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
