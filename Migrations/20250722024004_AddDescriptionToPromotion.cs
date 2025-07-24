using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DATN.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToPromotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột Description vào bảng Promotions
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Promotions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Mô tả khuyến mãi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa cột Description nếu rollback migration
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Promotions");
        }
    }

}
