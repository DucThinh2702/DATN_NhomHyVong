using Microsoft.AspNetCore.Mvc;
using DATN.Models;
using DATN.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DATN.Controllers
{
    public class ProductsController : Controller
    {
        private readonly PhotoService _photoService;

        public ProductsController(PhotoService photoService)
        {
            _photoService = photoService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var uploadResult = await _photoService.UploadPhotoAsync(file);
                model.HinhAnhDaiDien = uploadResult.SecureUrl.ToString();
            }

            // TODO: Lưu model vào CSDL (EF hoặc Dapper)
            // ví dụ: _context.Products.Add(model); _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
