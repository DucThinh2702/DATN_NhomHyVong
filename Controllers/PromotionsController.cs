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
            // Kiểm tra ModelState trước
            if (!ModelState.IsValid)
            {
                return View(promotion); // Nếu có lỗi, giữ lại view để người dùng chỉnh sửa
            }

            // Kiểm tra ngày bắt đầu và ngày kết thúc
            if (promotion.StartDate >= promotion.EndDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
            }

            // Kiểm tra giá trị giảm theo loại giảm giá
            if (promotion.PromoType == "Phần trăm" && (promotion.DiscountValue < 0 || promotion.DiscountValue > 100))
            {
                ModelState.AddModelError("DiscountValue", "Giá trị phần trăm phải nằm trong khoảng từ 0 đến 100.");
            }

            if (promotion.PromoType == "Số tiền cố định" && promotion.DiscountValue > promotion.MinOrderAmount)
            {
                ModelState.AddModelError("DiscountValue", "Giá trị giảm không được vượt quá giá trị đơn hàng tối thiểu.");
            }

            // Kiểm tra nếu tên mã giảm giá đã tồn tại
            if (await CheckPromoNameExists(promotion.PromoName))
            {
                ModelState.AddModelError("PromoName", "Tên mã giảm giá đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction("MaGiamGia", "Admin");
            }

            return View(promotion);
        }

        // Kiểm tra nếu tên mã giảm giá đã tồn tại trong cơ sở dữ liệu
        private async Task<bool> CheckPromoNameExists(string promoName)
        {
            return await _context.Promotions.AnyAsync(p => p.PromoName.ToLower() == promoName.ToLower());
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
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion); // Truyền dữ liệu để người dùng chỉnh sửa
        }

        // POST: Promotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PromoCode,PromoName,PromoType,DiscountValue,MinOrderAmount,StartDate,EndDate,Quantity,UsedQuantity,Status")] Promotion promotion)
        {
            if (id != promotion.PromoCode)
            {
                return NotFound();
            }

            // Kiểm tra ModelState trước
            if (!ModelState.IsValid)
            {
                return View(promotion); // Nếu có lỗi, giữ lại view để người dùng chỉnh sửa
            }

            // Kiểm tra ngày bắt đầu và ngày kết thúc
            if (promotion.StartDate >= promotion.EndDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
            }

            // Kiểm tra giá trị giảm theo loại giảm giá
            if (promotion.PromoType == "Phần trăm" && (promotion.DiscountValue < 0 || promotion.DiscountValue > 100))
            {
                ModelState.AddModelError("DiscountValue", "Giá trị phần trăm phải nằm trong khoảng từ 0 đến 100.");
            }

            if (promotion.PromoType == "Số tiền cố định" && promotion.DiscountValue > promotion.MinOrderAmount)
            {
                ModelState.AddModelError("DiscountValue", "Giá trị giảm không được vượt quá giá trị đơn hàng tối thiểu.");
            }

            // Kiểm tra nếu tên mã giảm giá đã tồn tại
            if (await CheckPromoNameExists(promotion.PromoName, id))
            {
                ModelState.AddModelError("PromoName", "Tên mã giảm giá đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promotion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promotion.PromoCode))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction("MaGiamGia", "Admin");
            }

            return View(promotion); // Nếu có lỗi, trả về view để người dùng chỉnh sửa
        }

        // Kiểm tra nếu tên mã giảm giá đã tồn tại trong cơ sở dữ liệu (sửa)
        private async Task<bool> CheckPromoNameExists(string promoName, int? id)
        {
            // Kiểm tra trùng tên mã giảm giá, ngoại trừ trường hợp đang chỉnh sửa mã giảm giá hiện tại
            return await _context.Promotions
                .AnyAsync(p => p.PromoName.ToLower() == promoName.ToLower() && p.PromoCode != id);
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.PromoCode == id);
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
    }
}
