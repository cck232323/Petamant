using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyDotnetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentCommentId",
                table: "Comments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReplyToUserId",
                table: "Comments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplyToUserName",
                table: "Comments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId",
                principalTable: "Comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ReplyToUserId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ReplyToUserName",
                table: "Comments");
        }
    }
}
