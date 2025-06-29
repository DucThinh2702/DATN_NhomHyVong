using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DATN.Models;
using DATN.Repositories;

namespace DATN.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderRepository _repo;

        public OrdersController(OrderRepository repo)
        {
            _repo = repo;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _repo.GetAllAsync();
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateAsync(order);
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                await _repo.UpdateAsync(order);
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var order = await _repo.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
