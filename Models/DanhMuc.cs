using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DATN.Models
{
    public class DanhMuc
    {
        [Key]
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }

        [NotMapped] // Rất quan trọng để EF bỏ qua property này
        public IFormFile HinhAnhFile { get; set; }

        public ICollection<SanPham> SanPhams { get; set; }
    }
}
