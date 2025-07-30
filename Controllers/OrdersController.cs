using Microsoft.AspNetCore.Mvc;
using DATN.Models;
using DATN.Data;

namespace DATN.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly DatnDbContext _context;

        public OrdersController(DatnDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Gán thêm giá trị mặc định
            order.OrderDate = DateTime.Now;
            order.PaymentStatus = string.IsNullOrEmpty(order.PaymentStatus) ? "Chưa thanh toán" : order.PaymentStatus;
            order.OrderStatus = string.IsNullOrEmpty(order.OrderStatus) ? "Chờ xử lý" : order.OrderStatus;

            // Tạm tách OrderDetails để tránh lỗi "cùng lúc add cả Order mới và chi tiết có ID"
            var details = order.OrderDetails.ToList();
            order.OrderDetails = null!;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in details)
            {
                item.OrderId = order.OrderId; // gán lại OrderId vừa tạo
                _context.OrderDetails.Add(item);
            }
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Đặt hàng thành công", orderId = order.OrderId });
        }
    }
}
