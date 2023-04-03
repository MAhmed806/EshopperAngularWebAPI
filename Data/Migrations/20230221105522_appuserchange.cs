using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShopperAngular.Migrations
{
    public partial class appuserchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RememberMe",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RememberMe",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }
    }
}
