//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using DATN.Data;
//using DATN.Models;

//namespace DATN.Controllers
//{
//    public class ProductController : Controller
//    {
//        private readonly DatnContext _context;

//        public ProductController(DatnContext context)
//        {
//            _context = context;
//        }

//        // GET: Product/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: Product/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Product product)
//        {
//            if (await _context.Products.AnyAsync(p => p.ProductName.ToLower() == product.ProductName.ToLower()))
//            {
//                ModelState.AddModelError("ProductName", "Tên sản phẩm đã tồn tại.");
//                return View(product);
//            }

//            if (ModelState.IsValid)
//            {
//                _context.Add(product);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }

//            return View(product);
//        }

//        // Index action để xem danh sách sản phẩm
//        public async Task<IActionResult> Index()
//        {
//            return View(await _context.Products.ToListAsync());
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DATN.Data;
using DATN.Models;
using System.Threading.Tasks;
using System.Linq;

public class ProductController : Controller
{
    private readonly DatnContext _context;

    public ProductController(DatnContext context)
    {
        _context = context;
    }

    // GET: Product/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Product/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            bool isDuplicate = await _context.Products
                .AnyAsync(p => p.ProductName.ToLower() == product.ProductName.ToLower());

            if (isDuplicate)
            {
                ModelState.AddModelError("ProductName", "Tên sản phẩm đã tồn tại.");
                return View(product);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    // GET: Product/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        return View(product);
    }

    // POST: Product/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.ProductId)
            return NotFound();

        if (ModelState.IsValid)
        {
            bool isDuplicate = await _context.Products
                .AnyAsync(p => p.ProductName.ToLower() == product.ProductName.ToLower()
                               && p.ProductId != product.ProductId);

            if (isDuplicate)
            {
                ModelState.AddModelError("ProductName", "Tên sản phẩm đã tồn tại.");
                return View(product);
            }

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.ProductId == product.ProductId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    // GET: Product
    public async Task<IActionResult> Index()
    {
        return View(await _context.Products.ToListAsync());
    }
    [HttpGet]
    public async Task<JsonResult> IsProductNameAvailable(string productName, int? id)
    {
        if (string.IsNullOrEmpty(productName))
            return Json(true); // Cho phép rỗng, validation sẽ kiểm tra chỗ khác

        var exists = await _context.Products
            .AnyAsync(p => p.ProductName.ToLower() == productName.ToLower() && p.ProductId != id);

        return Json(!exists); // Trả về true nếu KHÔNG tồn tại
    }

}
