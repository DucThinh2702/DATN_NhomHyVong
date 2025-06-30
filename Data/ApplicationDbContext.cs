using Microsoft.EntityFrameworkCore;

namespace DATN.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        // DbSet cho các bảng
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<KhuyenMai> KhuyenMais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========== Thiết lập khoá chính ==========
            modelBuilder.Entity<SanPham>().HasKey(x => x.MaSP);
            modelBuilder.Entity<DanhMuc>().HasKey(x => x.MaDanhMuc);
            modelBuilder.Entity<NguoiDung>().HasKey(x => x.MaND);
            modelBuilder.Entity<TaiKhoan>().HasKey(x => x.MaTK);
            modelBuilder.Entity<VaiTro>().HasKey(x => x.MaVT);
            modelBuilder.Entity<GioHang>().HasKey(x => x.MaGH);
            modelBuilder.Entity<ChiTietGioHang>().HasKey(x => x.MaCTGH);
            modelBuilder.Entity<DonHang>().HasKey(x => x.MaDH);
            modelBuilder.Entity<ChiTietDonHang>().HasKey(x => x.MaCTDH);
            modelBuilder.Entity<KhuyenMai>().HasKey(x => x.MaKM);

            // ========== Thiết lập quan hệ ==========
            // SanPham - DanhMuc
            modelBuilder.Entity<SanPham>()
                .HasOne(sp => sp.DanhMuc)
                .WithMany(dm => dm.SanPhams)
                .HasForeignKey(sp => sp.MaDanhMuc)
                .OnDelete(DeleteBehavior.Restrict);

            // TaiKhoan - NguoiDung
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(tk => tk.NguoiDung)
                .WithMany(nd => nd.TaiKhoans)
                .HasForeignKey(tk => tk.MaND)
                .OnDelete(DeleteBehavior.Restrict);

            // TaiKhoan - VaiTro
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(tk => tk.VaiTro)
                .WithMany(vt => vt.TaiKhoans)
                .HasForeignKey(tk => tk.MaVT)
                .OnDelete(DeleteBehavior.Restrict);

            // GioHang - NguoiDung
            modelBuilder.Entity<GioHang>()
                .HasOne(gh => gh.NguoiDung)
                .WithMany(nd => nd.GioHangs)
                .HasForeignKey(gh => gh.MaND)
                .OnDelete(DeleteBehavior.Restrict);

            // ChiTietGioHang - GioHang
            modelBuilder.Entity<ChiTietGioHang>()
                .HasOne(ct => ct.GioHang)
                .WithMany(gh => gh.ChiTietGioHangs)
                .HasForeignKey(ct => ct.MaGH)
                .OnDelete(DeleteBehavior.Cascade);

            // ChiTietGioHang - SanPham
            modelBuilder.Entity<ChiTietGioHang>()
                .HasOne(ct => ct.SanPham)
                .WithMany()
                .HasForeignKey(ct => ct.MaSP)
                .OnDelete(DeleteBehavior.Restrict);

            // DonHang - NguoiDung
            modelBuilder.Entity<DonHang>()
                .HasOne(dh => dh.NguoiDung)
                .WithMany(nd => nd.DonHangs)
                .HasForeignKey(dh => dh.MaND)
                .OnDelete(DeleteBehavior.Restrict);

            // ChiTietDonHang - DonHang
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(ct => ct.DonHang)
                .WithMany(dh => dh.ChiTietDonHangs)
                .HasForeignKey(ct => ct.MaDH)
                .OnDelete(DeleteBehavior.Cascade);

            // ChiTietDonHang - SanPham
            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(ct => ct.SanPham)
                .WithMany()
                .HasForeignKey(ct => ct.MaSP)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
