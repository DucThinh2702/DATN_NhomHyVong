using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using DATN.Data;
using DATN.Models;

namespace DATN.Repositories
{
    public class ProductRepository
    {
        private readonly DapperHelper _dapper;

        public ProductRepository(DapperHelper dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var sql = "SELECT * FROM Products WHERE TrangThai = N'Còn hàng'";
            using var conn = _dapper.CreateConnection();
            return await conn.QueryAsync<Product>(sql);
        }

        public async Task<Product> GetByIdAsync(string maSP)
        {
            var sql = "SELECT * FROM Products WHERE MaSP = @MaSP";
            using var conn = _dapper.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Product>(sql, new { MaSP = maSP });
        }
    }
}
