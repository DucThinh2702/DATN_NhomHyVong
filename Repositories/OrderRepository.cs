using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Threading.Tasks;
using DATN.Models;
using System.Data;
using DATN.Data;

namespace DATN.Repositories
{
    public class OrderRepository
    {
        private readonly DapperHelper _dapper;

        public OrderRepository(DapperHelper dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var sql = "SELECT * FROM Orders";
            using var conn = _dapper.CreateConnection();
            return await conn.QueryAsync<Order>(sql);
        }

        public async Task<Order> GetByIdAsync(string maDH)
        {
            var sql = "SELECT * FROM Orders WHERE MaDH = @MaDH";
            using var conn = _dapper.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Order>(sql, new { MaDH = maDH });
        }

        public async Task<int> CreateAsync(Order order)
        {
            var sql = @"
                INSERT INTO Orders (MaDH, MaKH, NgayDatHang, SoLuong, TongTien, MaPTTT, TrangThaiThanhToan, TrangThaiDonHang, TenNguoiNhan, SoDienThoaiNguoiNhan, DiaChiGiaoHang, GhiChu, MaGiamGia, PhiVanChuyen)
                VALUES (@MaDH, @MaKH, @NgayDatHang, @SoLuong, @TongTien, @MaPTTT, @TrangThaiThanhToan, @TrangThaiDonHang, @TenNguoiNhan, @SoDienThoaiNguoiNhan, @DiaChiGiaoHang, @GhiChu, @MaGiamGia, @PhiVanChuyen)
            ";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, order);
        }

        public async Task<int> UpdateAsync(Order order)
        {
            var sql = @"
                UPDATE Orders SET 
                    MaKH = @MaKH,
                    SoLuong = @SoLuong,
                    TongTien = @TongTien,
                    MaPTTT = @MaPTTT,
                    TrangThaiThanhToan = @TrangThaiThanhToan,
                    TrangThaiDonHang = @TrangThaiDonHang,
                    TenNguoiNhan = @TenNguoiNhan,
                    SoDienThoaiNguoiNhan = @SoDienThoaiNguoiNhan,
                    DiaChiGiaoHang = @DiaChiGiaoHang,
                    GhiChu = @GhiChu,
                    MaGiamGia = @MaGiamGia,
                    PhiVanChuyen = @PhiVanChuyen
                WHERE MaDH = @MaDH
            ";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, order);
        }

        public async Task<int> DeleteAsync(string maDH)
        {
            var sql = "DELETE FROM Orders WHERE MaDH = @MaDH";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, new { MaDH = maDH });
        }
    }
}
