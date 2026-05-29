using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceProject.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditProductAddQunatityProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "products");
        }
    }
}
