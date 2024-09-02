using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagmentSystem.Migrations
{
    /// <inheritdoc />
    public partial class activeforalltablesandNavigationPropertiesandBorrowsRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "BookBorrows",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MemberId",
                table: "BookBorrows",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookBorrows_BookId",
                table: "BookBorrows",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookBorrows_MemberId",
                table: "BookBorrows",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrows_Books_BookId",
                table: "BookBorrows",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrows_Members_MemberId",
                table: "BookBorrows",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrows_Books_BookId",
                table: "BookBorrows");

            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrows_Members_MemberId",
                table: "BookBorrows");

            migrationBuilder.DropIndex(
                name: "IX_BookBorrows_BookId",
                table: "BookBorrows");

            migrationBuilder.DropIndex(
                name: "IX_BookBorrows_MemberId",
                table: "BookBorrows");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "BookBorrows");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "BookBorrows");

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
    }
}
