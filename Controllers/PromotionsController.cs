using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Data;
using DATN.Models;

namespace DATN.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly DatnContext _context;

        public PromotionsController(DatnContext context)
        {
            _context = context;
        }

        // GET: Promotions/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Views/Promotions/Create.cshtml
        }

        // POST: Promotions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PromoCode,PromoName,PromoType,DiscountValue,MinOrderAmount,StartDate,EndDate,Quantity,UsedQuantity,Status")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promotion);
                await _context.SaveChangesAsync();

                // Quay về danh sách trong AdminController
                return RedirectToAction("MaGiamGia", "Admin");
            }

            return View(promotion); // Nếu lỗi, giữ lại view để hiển thị
        }

        // GET: Promotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var promotion = await _context.Promotions.FirstOrDefaultAsync(m => m.PromoCode == id);
            if (promotion == null) return NotFound();

            return View(promotion); // Views/Promotions/Details.cshtml
        }

        // GET: Promotions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) return NotFound();

            return View(promotion); // Views/Promotions/Edit.cshtml
        }

        // POST: Promotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PromoCode,PromoName,PromoType,DiscountValue,MinOrderAmount,StartDate,EndDate,Quantity,UsedQuantity,Status")] Promotion promotion)
        {
            if (id != promotion.PromoCode) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promotion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promotion.PromoCode)) return NotFound();
                    throw;
                }

                // Quay về danh sách trong AdminController
                return RedirectToAction("MaGiamGia", "Admin");
            }

            return View(promotion);
        }

        // GET: Promotions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var promotion = await _context.Promotions.FirstOrDefaultAsync(m => m.PromoCode == id);
            if (promotion == null) return NotFound();

            return View(promotion); // Views/Promotions/Delete.cshtml
        }

        // POST: Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
            }

            // Quay về danh sách trong AdminController
            return RedirectToAction("MaGiamGia", "Admin");
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.PromoCode == id);
        }
    }
}
