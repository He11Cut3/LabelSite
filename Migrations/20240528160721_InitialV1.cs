using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabelSite.Migrations
{
    /// <inheritdoc />
    public partial class InitialV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesDatas_AspNetUsers_UserId1",
                table: "SalesDatas");

            migrationBuilder.DropIndex(
                name: "IX_SalesDatas_UserId1",
                table: "SalesDatas");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "SalesDatas");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "SalesDatas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_SalesDatas_UserId",
                table: "SalesDatas",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesDatas_AspNetUsers_UserId",
                table: "SalesDatas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesDatas_AspNetUsers_UserId",
                table: "SalesDatas");

            migrationBuilder.DropIndex(
                name: "IX_SalesDatas_UserId",
                table: "SalesDatas");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "SalesDatas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "SalesDatas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesDatas_UserId1",
                table: "SalesDatas",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesDatas_AspNetUsers_UserId1",
                table: "SalesDatas",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
