using data.Concrate;
using dto.viewmodels;
using entity.Concrate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web.Areas.Juri.Controllers
{
	[Area("Juri")]
	public class Degerlendir : Controller
	{
		Context Context = new Context();

		public IActionResult Index()
		{
			var userTc = User.Identity.Name;
			var personelId = Context.Personels
				.Where(p => p.TC == userTc)
				.Select(p => p.Personel_Id)
				.FirstOrDefault();

			var model = Context.BasvuruYonlendirs
				.Include(x => x.Basvuru)
					.ThenInclude(b => b.Personel)
				.Include(x => x.Basvuru)
					.ThenInclude(b => b.Ilan)
				.Where(x => x.Personel_Id == personelId)
				.Select(x => new BasvuruJuriViewModel
				{
					BasvuruId = x.Basvuru_Id,
					Isim = x.Basvuru.Personel.Isim,
					Soyisim = x.Basvuru.Personel.Soyisim,
					Eposta = x.Basvuru.Personel.Eposta,
					Telefon = x.Basvuru.Personel.Telefon,
					IlanBaslik = x.Basvuru.Ilan.Baslik,
					DosyaYolu = Context.DegerlendirmeBelges
						.Where(d => d.Basvuru_Id == x.Basvuru_Id && d.Personel_Id == personelId)
						.Select(d => d.DosyaYolu)
						.FirstOrDefault()
				})
				.ToList();

			return View(model);
		}

		[HttpPost]
		public IActionResult BelgeYukle(IFormFile dosya, int basvuruId)
		{
			Console.WriteLine("BelgeYukle çalıştı!");
			Console.WriteLine($"Dosya: {dosya?.FileName}, BaşvuruId: {basvuruId}");
			var userTc = User.Identity.Name;
			var personelId = Context.Personels
				.Where(p => p.TC == userTc)
				.Select(p => p.Personel_Id)
				.FirstOrDefault();

			if (dosya != null && dosya.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dosyalar");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var fileName = Guid.NewGuid() + Path.GetExtension(dosya.FileName);
				var filePath = Path.Combine(uploadsFolder, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					dosya.CopyTo(stream);
				}

				var belge = new DegerlendirmeBelge
				{
					Basvuru_Id = basvuruId,
					Personel_Id = personelId,
					DosyaYolu = "/dosyalar/" + fileName
				};

				Context.DegerlendirmeBelges.Add(belge);
				Context.SaveChanges();
			}

			return RedirectToAction("Index");
		}
	}


}
