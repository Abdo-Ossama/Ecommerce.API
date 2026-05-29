using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceProject.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LastubdateindbmodelsAddNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ListPrice",
                table: "Carts",
                type: "float",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<double>(
                name: "PricePerProduct",
                table: "Carts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerProduct",
                table: "Carts");

            migrationBuilder.AlterColumn<long>(
                name: "ListPrice",
                table: "Carts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
