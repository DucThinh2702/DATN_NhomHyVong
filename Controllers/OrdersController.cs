using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DATN.Repositories;
using DATN.Models;

namespace DATN.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderRepository _orderRepo;
        private readonly OrderDetailRepository _orderDetailRepo;

        public OrdersController(OrderRepository orderRepo, OrderDetailRepository orderDetailRepo)
        {
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
        }

        // GET: /Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepo.GetAllAsync();
            return View(orders);
        }

        // GET: /Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var details = await _orderDetailRepo.GetByOrderIdAsync(id);
            ViewBag.Order = order;
            return View(details);
        }

        //// GET: /Orders/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var order = await _orderRepo.GetByIdAsync(id);
        //    if (order == null)
        //        return NotFound();
        //    return View(order);
        //}

        //// POST: /Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    // Xoá chi tiết đơn hàng trước
        //    await _orderDetailRepo.DeleteByOrderIdAsync(id);  

        //    // Xoá đơn hàng
        //    await _orderRepo.DeleteAsync(id);

        //    return RedirectToAction(nameof(Index));
        //}


        //// GET: /Orders/Create
        //public IActionResult Create()
        //{
        //    var model = new OrderCreateViewModel
        //    {
        //        Order = new Order(),
        //        OrderDetails = new List<OrderDetail>()
        //    };
        //    return View(model);
        //}

        //// POST: /Orders/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(OrderCreateViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _orderRepo.CreateAsync(model.Order);
        //        foreach (var detail in model.OrderDetails)
        //        {
        //            detail.OrderID = model.Order.OrderID;
        //            await _orderDetailRepo.CreateAsync(detail);
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(model);
        //}

        // GET: /Orders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            var details = await _orderDetailRepo.GetByOrderIdAsync(id);
            if (order == null) return NotFound();

            var model = new OrderCreateViewModel
            {
                Order = order,
                OrderDetails = details.ToList()
            };
            return View(model);
        }

        // POST: /Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // giữ nguyên nếu lỗi
            }

            // lấy đơn hàng gốc từ database
            var existingOrder = await _orderRepo.GetByIdAsync(model.Order.OrderID);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // chỉ cập nhật các trường cho phép sửa
            existingOrder.RecipientName = model.Order.RecipientName;
            existingOrder.RecipientPhone = model.Order.RecipientPhone;
            existingOrder.DeliveryAddress = model.Order.DeliveryAddress;
            existingOrder.OrderStatus = model.Order.OrderStatus;
            existingOrder.PaymentStatus = model.Order.PaymentStatus;
            existingOrder.Note = model.Order.Note;

            await _orderRepo.UpdateAsync(existingOrder);
            return RedirectToAction(nameof(Index));
        }



    }
}
