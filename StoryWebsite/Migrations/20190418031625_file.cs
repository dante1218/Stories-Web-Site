using Microsoft.EntityFrameworkCore.Migrations;

namespace StoryWebsite.Data.Migrations
{
    public partial class file : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoryId",
                table: "Texts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Texts_StoryId",
                table: "Texts",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Texts_Stories_StoryId",
                table: "Texts",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "StoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Texts_Stories_StoryId",
                table: "Texts");

            migrationBuilder.DropIndex(
                name: "IX_Texts_StoryId",
                table: "Texts");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Texts");
        }
    }
}
