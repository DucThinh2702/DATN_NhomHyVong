using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATN.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShippingProviderNameInPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ShippingProviders",
                newName: "ShippingProviderName");

            migrationBuilder.AddColumn<string>(
                name: "ShippingProviderName",
                table: "Promotions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingProviderName",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "ShippingProviderName",
                table: "ShippingProviders",
                newName: "Name");
        }
    }
}
