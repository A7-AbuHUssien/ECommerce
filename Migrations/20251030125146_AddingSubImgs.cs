using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddingSubImgs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSubImages",
                table: "ProductSubImages");

            migrationBuilder.AlterColumn<string>(
                name: "MainImg",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSubImages",
                table: "ProductSubImages",
                columns: new[] { "Img", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubImages_ProductId",
                table: "ProductSubImages",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSubImages",
                table: "ProductSubImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductSubImages_ProductId",
                table: "ProductSubImages");

            migrationBuilder.AlterColumn<string>(
                name: "MainImg",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSubImages",
                table: "ProductSubImages",
                columns: new[] { "ProductId", "Img" });
        }
    }
}
