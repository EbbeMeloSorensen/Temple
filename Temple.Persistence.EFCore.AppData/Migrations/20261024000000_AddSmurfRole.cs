using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Temple.Persistence.EFCore.AppData.Migrations
{
    public partial class AddSmurfRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Smurfs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Smurfs");
        }
    }
}
