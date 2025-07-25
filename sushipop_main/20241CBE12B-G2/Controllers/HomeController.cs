using _20241CBE12B_G2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace _20241CBE12B_G2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbContext _context;

        public HomeController(ILogger<HomeController> logger, DbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // nro dia y nombre
            var nroDia = (int)DateTime.Today.DayOfWeek;

            string dia = System.Globalization.CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetDayName((DayOfWeek)nroDia);

            var descuento = await _context.Descuento.Include(d => d.Producto).Where(d => d.Dia == nroDia && d.Activo).FirstOrDefaultAsync();

            HomeViewModel vm = new()
            {
                Dia = dia
            };

            if (descuento == null)
            {
                vm.MensajePromo = "Hoy es " + dia + ". Disfruta del mejor sushi #EnCasa con amigos.";
            }
            else
            {
                vm.Porcentaje = descuento.Porcentaje + "%";
                vm.Producto = descuento.Producto.Nombre;
            }

            switch (nroDia)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    vm.HorarioAtencion = "de 19 a 23 hs.";
                    break;
                case 5:
                case 6:
                case 7:
                    vm.HorarioAtencion = "de 11 a 14 hs. y de 19 a 23 hs.";
                    break;
            }

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
