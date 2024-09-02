using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class activeforalltablesandNavigationProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId1",
                table: "BookBorrows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MemberId1",
                table: "BookBorrows",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookBorrows_BookId1",
                table: "BookBorrows",
                column: "BookId1");

            migrationBuilder.CreateIndex(
                name: "IX_BookBorrows_MemberId1",
                table: "BookBorrows",
                column: "MemberId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrows_Books_BookId1",
                table: "BookBorrows",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrows_Members_MemberId1",
                table: "BookBorrows",
                column: "MemberId1",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrows_Books_BookId1",
                table: "BookBorrows");

            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrows_Members_MemberId1",
                table: "BookBorrows");

            migrationBuilder.DropIndex(
                name: "IX_BookBorrows_BookId1",
                table: "BookBorrows");

            migrationBuilder.DropIndex(
                name: "IX_BookBorrows_MemberId1",
                table: "BookBorrows");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "BookBorrows");

            migrationBuilder.DropColumn(
                name: "MemberId1",
                table: "BookBorrows");
        }
    }
}
