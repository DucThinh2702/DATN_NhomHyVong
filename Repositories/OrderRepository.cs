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

        public async Task<Order> GetByIdAsync(int orderId)
        {
            var sql = "SELECT * FROM Orders WHERE OrderID = @OrderID";
            using var conn = _dapper.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Order>(sql, new { OrderID = orderId });
        }

        public async Task<int> CreateAsync(Order order)
        {
            var sql = @"
                INSERT INTO Orders (
                    UserID, OrderDate, Quantity, TotalAmount, PaymentMethodID, 
                    PaymentStatus, OrderStatus, RecipientName, RecipientPhone, 
                    DeliveryAddress, Note, DiscountCode, ShippingFee
                )
                VALUES (
                    @UserID, @OrderDate, @Quantity, @TotalAmount, @PaymentMethodID, 
                    @PaymentStatus, @OrderStatus, @RecipientName, @RecipientPhone, 
                    @DeliveryAddress, @Note, @DiscountCode, @ShippingFee
                )";

            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, order);
        }

        public async Task<int> UpdateAsync(Order order)
        {
            var sql = @"
        UPDATE Orders SET 
            RecipientName = @RecipientName,
            RecipientPhone = @RecipientPhone,
            DeliveryAddress = @DeliveryAddress,
            OrderStatus = @OrderStatus,
            PaymentStatus = @PaymentStatus,
            Note = @Note
        WHERE OrderID = @OrderID";

            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, order);
        }


        public async Task<int> DeleteAsync(int orderId)
        {
            var sql = "DELETE FROM Orders WHERE OrderID = @OrderID";
            using var conn = _dapper.CreateConnection();
            return await conn.ExecuteAsync(sql, new { OrderID = orderId });
        }
    }
}