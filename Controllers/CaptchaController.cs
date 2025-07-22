using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;

namespace DATN.Controllers
{
    public class CaptchaController : Controller
    {
        private const int CaptchaWidth = 120;
        private const int CaptchaHeight = 40;
        private const int CaptchaLength = 5;
        private const string CaptchaChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789abcdef";

        [HttpGet]
        public IActionResult Generate()
        {
            string code = GenerateCaptchaCode(CaptchaLength);
            HttpContext.Session.SetString("CaptchaCode", code);

            using var image = new Image<Rgba32>(CaptchaWidth, CaptchaHeight);
            image.Mutate(ctx =>
            {
                ctx.Fill(Color.White);
                DrawNoiseLines(ctx);
                DrawCaptchaText(ctx, code);
            });

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            return File(ms.ToArray(), "image/png");
        }

        private static readonly Random _rand = new();

        private static string GenerateCaptchaCode(int length)
        {
            return new string(Enumerable.Repeat(CaptchaChars, length)
                .Select(s => s[_rand.Next(s.Length)]).ToArray() ?? []);
        }


        private static void DrawCaptchaText(IImageProcessingContext ctx, string code)
        {
            var fontFamily = SystemFonts.Families.First();
            var font = fontFamily.CreateFont(22, FontStyle.Bold);

            int spacing = 18;
            for (int i = 0; i < code.Length; i++)
            {
                var text = code[i].ToString();
                float x = 10 + i * spacing;
                float y = 5 + (i % 2 == 0 ? 2 : -2);

                ctx.DrawText(text, font, Color.Black, new PointF(x, y));
            }
        }

        private static void DrawNoiseLines(IImageProcessingContext ctx)
        {
            var rand = new Random();
            var penColor = Color.LightGray;

            for (int i = 0; i < 5; i++)
            {
                var p1 = new PointF(rand.Next(CaptchaWidth), rand.Next(CaptchaHeight));
                var p2 = new PointF(rand.Next(CaptchaWidth), rand.Next(CaptchaHeight));
                _ = ctx.DrawLine(penColor, 1, [p1, p2]);
            }
        }
    }
}
