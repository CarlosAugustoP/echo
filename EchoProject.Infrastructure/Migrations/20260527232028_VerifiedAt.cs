using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VerifiedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "verified_at",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verified_at",
                table: "users");
        }
    }
}
