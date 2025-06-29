using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DATN.Models;
using DATN.Repositories;

namespace DATN.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly OrderDetailRepository _repo;

        public OrderDetailsController(OrderDetailRepository repo)
        {
            _repo = repo;
        }

        // GET: OrderDetails/ByOrder/{maDH}
        public async Task<IActionResult> ByOrder(string maDH)
        {
            var details = await _repo.GetByOrderIdAsync(maDH);
            ViewBag.MaDH = maDH;
            return View(details);
        }

        // GET: OrderDetails/Create
        public IActionResult Create(string maDH)
        {
            var model = new OrderDetail { MaDH = maDH };
            return View(model);
        }

        // POST: OrderDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetail detail)
        {
            if (ModelState.IsValid)
            {
                await _repo.CreateAsync(detail);
                return RedirectToAction("ByOrder", new { maDH = detail.MaDH });
            }
            return View(detail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var detail = await _repo.GetByIdAsync(id);
            if (detail == null) return NotFound();
            return View(detail);
        }

        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderDetail detail)
        {
            if (ModelState.IsValid)
            {
                await _repo.UpdateAsync(detail);
                return RedirectToAction("ByOrder", new { maDH = detail.MaDH });
            }
            return View(detail);
        }

        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var detail = await _repo.GetByIdAsync(id);
            if (detail == null) return NotFound();
            return View(detail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var detail = await _repo.GetByIdAsync(id);
            if (detail != null)
            {
                await _repo.DeleteAsync(id);
                return RedirectToAction("ByOrder", new { maDH = detail.MaDH });
            }
            return NotFound();
        }
    }
}
