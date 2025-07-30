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


    }
}
