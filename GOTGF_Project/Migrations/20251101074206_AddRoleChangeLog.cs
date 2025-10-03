using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GOTGF_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleChangeLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleChangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldRoles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleChangeLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleChangeLogs");
        }
    }
}
