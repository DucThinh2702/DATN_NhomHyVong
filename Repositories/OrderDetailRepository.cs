using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using DATN.Data;
using DATN.Models;

namespace DATN.Repositories
{
    public class OrderDetailRepository
    {
        private readonly DapperHelper _dapper;

        public OrderDetailRepository(DapperHelper dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            var sql = "SELECT * FROM OrderDetails WHERE OrderID = @OrderID";
            using var conn = _dapper.CreateConnection();
            return await conn.QueryAsync<OrderDetail>(sql, new { OrderID = orderId });
        }

        public async Task<OrderDetail> GetByIdAsync(int orderDetailId)
        {
            var sql = "SELECT * FROM OrderDetails WHERE OrderDetailID = @OrderDetailID";
            using var conn = _dapper.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<OrderDetail>(sql, new { OrderDetailID = orderDetailId });
        }

        public async Task<int> CreateAsync(OrderDetail detail)
        {
            var sql = @"
                INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice, TotalPrice)
                VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice, @TotalPrice)
            ";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, detail);
        }

        public async Task<int> UpdateAsync(OrderDetail detail)
        {
            var sql = @"
                UPDATE OrderDetails SET 
                    ProductID = @ProductID,
                    Quantity = @Quantity,
                    UnitPrice = @UnitPrice,
                    TotalPrice = @TotalPrice
                WHERE OrderDetailID = @OrderDetailID
            ";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, detail);
        }
        public async Task<int> DeleteByOrderIdAsync(int orderId)
        {
            using var conn = _dapper.CreateConnection();
            string sql = "DELETE FROM OrderDetails WHERE OrderID = @OrderID";
            return await conn.ExecuteAsync(sql, new { OrderID = orderId });
        }

        public async Task<int> DeleteAsync(int orderDetailId)
        {
            var sql = "DELETE FROM OrderDetails WHERE OrderDetailID = @OrderDetailID";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, new { OrderDetailID = orderDetailId });
        }
    }
}
