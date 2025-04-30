using data.Concrate;
using dto.viewmodels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace web.Areas.Juri.Controllers
{
	[Area("Juri")]
	public class HomeController : Controller
	{
		Context Context = new Context();
        public IActionResult Index()
        {
            var model = Context.BasvuruYonlendirs
        .Include(x => x.Basvuru)
            .ThenInclude(b => b.Personel)
        .Include(x => x.Basvuru)
            .ThenInclude(b => b.Ilan)
        .Where(x => x.Basvuru != null && x.Basvuru.Personel != null && x.Basvuru.Ilan != null)
        .Select(x => new BasvuruJuriViewModel
        {
            Isim = x.Basvuru.Personel.Isim,
            Soyisim = x.Basvuru.Personel.Soyisim,
            Eposta = x.Basvuru.Personel.Eposta,
            Telefon = x.Basvuru.Personel.Telefon,
            IlanBaslik = x.Basvuru.Ilan.Baslik
        })
        .ToList();

            return View(model);
        }

    }
}