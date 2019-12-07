using Microsoft.EntityFrameworkCore.Migrations;

namespace StoryWebsite.Data.Migrations
{
    public partial class image : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoryId",
                table: "Images",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_StoryId",
                table: "Images",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Stories_StoryId",
                table: "Images",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "StoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Stories_StoryId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_StoryId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Images");
        }
    }
}
