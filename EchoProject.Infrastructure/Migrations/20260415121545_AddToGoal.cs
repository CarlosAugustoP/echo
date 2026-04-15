using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddToGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "goals",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "goals");
        }
    }
}
