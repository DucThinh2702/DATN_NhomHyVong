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
    }
}
