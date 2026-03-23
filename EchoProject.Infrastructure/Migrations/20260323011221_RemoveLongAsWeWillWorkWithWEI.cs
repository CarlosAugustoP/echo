using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLongAsWeWillWorkWithWEI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalCost",
                table: "donations",
                newName: "total_cost");

            migrationBuilder.AlterColumn<decimal>(
                name: "target_amount",
                table: "goals",
                type: "numeric(38,18)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "current_amount",
                table: "goals",
                type: "numeric(38,18)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 0L);

            migrationBuilder.AlterColumn<decimal>(
                name: "CostPerUnit",
                table: "goals",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "donations",
                type: "numeric(38,18)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_cost",
                table: "donations",
                type: "numeric(38,18)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_users_tax_id",
                table: "users",
                column: "tax_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_tax_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "total_cost",
                table: "donations",
                newName: "TotalCost");

            migrationBuilder.AlterColumn<long>(
                name: "target_amount",
                table: "goals",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,18)");

            migrationBuilder.AlterColumn<long>(
                name: "current_amount",
                table: "goals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,18)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<long>(
                name: "CostPerUnit",
                table: "goals",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "amount",
                table: "donations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,18)");

            migrationBuilder.AlterColumn<long>(
                name: "TotalCost",
                table: "donations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(38,18)");
        }
    }
}
