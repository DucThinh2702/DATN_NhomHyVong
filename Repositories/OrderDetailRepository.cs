    using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DATN.Data;
using DATN.Models;
using System.Data;

namespace DATN.Repositories
{
    public class OrderDetailRepository
    {
        private readonly DapperHelper _dapper;

        public OrderDetailRepository(DapperHelper dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(string maDH)
        {
            var sql = "SELECT * FROM OrderDetails WHERE MaDH = @MaDH";
            using var conn = _dapper.CreateConnection();
            return await conn.QueryAsync<OrderDetail>(sql, new { MaDH = maDH });
        }

        public async Task<OrderDetail> GetByIdAsync(string maCTDH)
        {
            var sql = "SELECT * FROM OrderDetails WHERE MaCTDH = @MaCTDH";
            using var conn = _dapper.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<OrderDetail>(sql, new { MaCTDH = maCTDH });
        }

        public async Task<int> CreateAsync(OrderDetail detail)
        {
            var sql = @"
                INSERT INTO OrderDetails (MaCTDH, MaDH, MaSP, SoLuong, DonGia, ThanhTien)
                VALUES (@MaCTDH, @MaDH, @MaSP, @SoLuong, @DonGia, @ThanhTien)
            ";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, detail);
        }

        public async Task<int> UpdateAsync(OrderDetail detail)
        {
            var sql = @"
                UPDATE OrderDetails SET 
                    MaSP = @MaSP,
                    SoLuong = @SoLuong,
                    DonGia = @DonGia,
                    ThanhTien = @ThanhTien
                WHERE MaCTDH = @MaCTDH
            ";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, detail);
        }

        public async Task<int> DeleteAsync(string maCTDH)
        {
            var sql = "DELETE FROM OrderDetails WHERE MaCTDH = @MaCTDH";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, new { MaCTDH = maCTDH });
        }
    }
}
