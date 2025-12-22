using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrgStructureApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmployeeRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
