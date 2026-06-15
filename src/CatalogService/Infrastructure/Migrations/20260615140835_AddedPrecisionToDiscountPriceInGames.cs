using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPrecisionToDiscountPriceInGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPrice",
                table: "Games",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                computedColumnSql: "\"Price\" - (\"Price\" * \"Discount\" / 100)",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldComputedColumnSql: "\"Price\" - (\"Price\" * \"Discount\" / 100)",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPrice",
                table: "Games",
                type: "numeric",
                nullable: false,
                computedColumnSql: "\"Price\" - (\"Price\" * \"Discount\" / 100)",
                stored: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldComputedColumnSql: "\"Price\" - (\"Price\" * \"Discount\" / 100)",
                oldStored: true);
        }
    }
}
