using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateNewVersi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FundsReleaseHash",
                table: "donations",
                newName: "funds_release_hash");

            migrationBuilder.AlterColumn<string>(
                name: "funds_release_hash",
                table: "donations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "funds_release_hash",
                table: "donations",
                newName: "FundsReleaseHash");

            migrationBuilder.AlterColumn<string>(
                name: "FundsReleaseHash",
                table: "donations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
