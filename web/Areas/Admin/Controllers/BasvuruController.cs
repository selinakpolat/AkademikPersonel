using data.Concrate;
using entity.Concrate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class BasvuruController : Controller
	{
		Context Context = new Context();
		[HttpGet]
		public IActionResult Index()
		{
			var degerler = Context.Basvurus
			.Include(b => b.Ilan)
			.Include(b => b.BasvuruStatu)
			.Include(b => b.Personel)
			.ThenInclude(b => b.Unvan)
			.OrderBy(b => b.Ilan_Id)
			.ToList();
			return View(degerler);
		}
		[HttpPost]
		public IActionResult BasvuruDelete(int id)
		{
			var silinecek = Context.Basvurus.Find(id);
			if (silinecek != null)
			{
				Context.Basvurus.Remove(silinecek);
				Context.SaveChanges();
				return Json(new { success = true });
			}
			return Json(new { success = false });
		}
        [HttpGet]
        public IActionResult BasvuruGet(int id)
        {
            var basvuru = Context.Basvurus
                .Include(b => b.Personel)
                .FirstOrDefault(b => b.Basvuru_Id == id);

            if (basvuru == null || basvuru.Personel == null)
            {
                return Json(null);
            }

            return Json(new
            {
                Personel_TC = basvuru.Personel.TC,
                Personel_Eposta = basvuru.Personel.Eposta,
                Personel_Telefon = basvuru.Personel.Telefon
            });
        }


        [HttpPost]
        public IActionResult BasvuruUpdate([FromBody] Basvuru c)
        {
            if (c == null)
            {
                return Json(new { success = false, message = "Gelen veri null" });
            }

            var guncellenecek = Context.Basvurus.Find(c.Basvuru_Id);
            if (guncellenecek != null)
            {
                guncellenecek.BasvuruStatu_Id = c.BasvuruStatu_Id;
                guncellenecek.Ilan_Id = c.Ilan_Id;
                guncellenecek.Personel_Id = c.Personel_Id;
                Context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Başvuru bulunamadı" });
        }


        [HttpGet]
        public IActionResult IlanDetay(int id) // bu id aslında Basvuru_Id olmalı
        {
            var basvuru = Context.Basvurus
                .Include(x => x.Ilan)
                .FirstOrDefault(x => x.Basvuru_Id == id);

            if (basvuru == null || basvuru.Ilan == null)
            {
                return Json(null);
            }

            return Json(new
            {
                Baslik = basvuru.Ilan.Baslik,
                Aciklama = basvuru.Ilan.Aciklama,
                Tarih = basvuru.Ilan.Tarih.ToString("dd.MM.yyyy")
            });
        }


    }
}
