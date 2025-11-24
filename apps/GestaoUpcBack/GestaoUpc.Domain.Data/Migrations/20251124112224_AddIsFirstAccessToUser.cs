using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoUpc.Domain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFirstAccessToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFirstAccess",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFirstAccess",
                table: "Users");
        }
    }
}
