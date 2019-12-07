using Microsoft.EntityFrameworkCore.Migrations;

namespace StoryWebsite.Data.Migrations
{
    public partial class slide2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Slides",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Slides");
        }
    }
}
