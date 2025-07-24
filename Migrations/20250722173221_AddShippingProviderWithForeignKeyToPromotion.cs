using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATN.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingProviderWithForeignKeyToPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingProviders",
                columns: table => new
                {
                    ShippingProviderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PromoCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingProviders", x => x.ShippingProviderId);
                    table.ForeignKey(
                        name: "FK_ShippingProviders_Promotions_PromoCode",
                        column: x => x.PromoCode,
                        principalTable: "Promotions",
                        principalColumn: "PromoCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingProviders_PromoCode",
                table: "ShippingProviders",
                column: "PromoCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingProviders");

            migrationBuilder.AddColumn<string>(
                name: "ShippingProvider",
                table: "Promotions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
