using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VendorFieldsAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionDate",
                table: "vendors",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type_item_supply",
                table: "vendors",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "CostPerUnit",
                table: "goals",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TotalCost",
                table: "donations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "transferred_to_vendor_id",
                table: "donations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_donations_transferred_to_vendor_id",
                table: "donations",
                column: "transferred_to_vendor_id");

            migrationBuilder.AddForeignKey(
                name: "FK_donations_vendors_transferred_to_vendor_id",
                table: "donations",
                column: "transferred_to_vendor_id",
                principalTable: "vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_donations_vendors_transferred_to_vendor_id",
                table: "donations");

            migrationBuilder.DropIndex(
                name: "IX_donations_transferred_to_vendor_id",
                table: "donations");

            migrationBuilder.DropColumn(
                name: "DecisionDate",
                table: "vendors");

            migrationBuilder.DropColumn(
                name: "type_item_supply",
                table: "vendors");

            migrationBuilder.DropColumn(
                name: "CostPerUnit",
                table: "goals");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "donations");

            migrationBuilder.DropColumn(
                name: "transferred_to_vendor_id",
                table: "donations");
        }
    }
}
