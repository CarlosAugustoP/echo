using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EchoProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "goal_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goal_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tax_id = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    zip_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address_Neighborhood = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    state = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    wallet_address = table.Column<string>(type: "character varying(42)", maxLength: 42, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_projects_users_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tax_id = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    wallet_address = table.Column<string>(type: "character varying(42)", maxLength: 42, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    approved_by_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vendors_users_approved_by_id",
                        column: x => x.approved_by_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "goals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    GoalTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    target_amount = table.Column<long>(type: "bigint", nullable: false),
                    current_amount = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_goals_goal_types_GoalTypeId",
                        column: x => x.GoalTypeId,
                        principalTable: "goal_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_goals_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "donations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    donor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    goal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    transaction_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_donations", x => x.id);
                    table.ForeignKey(
                        name: "FK_donations_goals_goal_id",
                        column: x => x.goal_id,
                        principalTable: "goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_donations_users_donor_id",
                        column: x => x.donor_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "goal_vendor",
                columns: table => new
                {
                    goal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goal_vendor", x => new { x.goal_id, x.vendor_id });
                    table.ForeignKey(
                        name: "FK_goal_vendor_goals_goal_id",
                        column: x => x.goal_id,
                        principalTable: "goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_goal_vendor_vendors_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_donations_donor_id",
                table: "donations",
                column: "donor_id");

            migrationBuilder.CreateIndex(
                name: "IX_donations_goal_id",
                table: "donations",
                column: "goal_id");

            migrationBuilder.CreateIndex(
                name: "IX_donations_transaction_hash",
                table: "donations",
                column: "transaction_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_goal_types_name",
                table: "goal_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_goal_vendor_vendor_id",
                table: "goal_vendor",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "IX_goals_GoalTypeId",
                table: "goals",
                column: "GoalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_goals_ProjectId",
                table: "goals",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_projects_ManagerId",
                table: "projects",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vendors_approved_by_id",
                table: "vendors",
                column: "approved_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_vendors_tax_id",
                table: "vendors",
                column: "tax_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "donations");

            migrationBuilder.DropTable(
                name: "goal_vendor");

            migrationBuilder.DropTable(
                name: "goals");

            migrationBuilder.DropTable(
                name: "vendors");

            migrationBuilder.DropTable(
                name: "goal_types");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
