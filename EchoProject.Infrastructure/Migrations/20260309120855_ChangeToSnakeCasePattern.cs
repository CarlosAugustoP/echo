using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToSnakeCasePattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_goals_goal_types_GoalTypeId",
                table: "goals");

            migrationBuilder.DropForeignKey(
                name: "FK_goals_projects_ProjectId",
                table: "goals");

            migrationBuilder.DropForeignKey(
                name: "FK_projects_users_ManagerId",
                table: "projects");

            migrationBuilder.RenameColumn(
                name: "Address_Neighborhood",
                table: "users",
                newName: "neighbourhood");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "projects",
                newName: "manager_id");

            migrationBuilder.RenameIndex(
                name: "IX_projects_ManagerId",
                table: "projects",
                newName: "IX_projects_manager_id");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "goals",
                newName: "project_id");

            migrationBuilder.RenameColumn(
                name: "GoalTypeId",
                table: "goals",
                newName: "goal_type_id");

            migrationBuilder.RenameIndex(
                name: "IX_goals_ProjectId",
                table: "goals",
                newName: "IX_goals_project_id");

            migrationBuilder.RenameIndex(
                name: "IX_goals_GoalTypeId",
                table: "goals",
                newName: "IX_goals_goal_type_id");

            migrationBuilder.AlterColumn<string>(
                name: "neighbourhood",
                table: "users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "goal_type_id",
                table: "goals",
                column: "goal_type_id",
                principalTable: "goal_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "project_id",
                table: "goals",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "manager_id",
                table: "projects",
                column: "manager_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "goal_type_id",
                table: "goals");

            migrationBuilder.DropForeignKey(
                name: "project_id",
                table: "goals");

            migrationBuilder.DropForeignKey(
                name: "manager_id",
                table: "projects");

            migrationBuilder.RenameColumn(
                name: "neighbourhood",
                table: "users",
                newName: "Address_Neighborhood");

            migrationBuilder.RenameColumn(
                name: "manager_id",
                table: "projects",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_projects_manager_id",
                table: "projects",
                newName: "IX_projects_ManagerId");

            migrationBuilder.RenameColumn(
                name: "project_id",
                table: "goals",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "goal_type_id",
                table: "goals",
                newName: "GoalTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_goals_project_id",
                table: "goals",
                newName: "IX_goals_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_goals_goal_type_id",
                table: "goals",
                newName: "IX_goals_GoalTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "Address_Neighborhood",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_goals_goal_types_GoalTypeId",
                table: "goals",
                column: "GoalTypeId",
                principalTable: "goal_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_goals_projects_ProjectId",
                table: "goals",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_projects_users_ManagerId",
                table: "projects",
                column: "ManagerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
