using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CostNowSnakeCasePrecisionUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CostPerUnit",
                table: "goals",
                newName: "cost_per_unit");

            migrationBuilder.AlterColumn<decimal>(
                name: "cost_per_unit",
                table: "goals",
                type: "numeric(38,18)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cost_per_unit",
                table: "goals",
                newName: "CostPerUnit");

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPerUnit",
                table: "goals",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,18)",
                oldNullable: true);
        }
    }
}
