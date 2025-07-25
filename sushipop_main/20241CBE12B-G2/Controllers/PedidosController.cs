using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _20241CBE12B_G2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace _20241CBE12B_G2.Controllers
{
    public class PedidosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PedidosController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Pedidos
        [Authorize(Roles = "EMPLEADO, ADMIN, CLIENTE")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("EMPLEADO"))
            {
                var pedidos = await _context.Pedido.Where(p => p.Estado != 5 || p.Estado != 6).ToListAsync();
                return View(pedidos);
            }

            var user = await _userManager.GetUserAsync(User);


            var pedidosCliente = await _context.Pedido
                                            .Include(p => p.Carrito)
                                            .ThenInclude(c => c.Cliente)
                                            .Where(p => p.Carrito.Cliente.Email == user.Email
                                            && p.FechaCompra >= DateTime.Now.AddDays(-90))
                                            .ToListAsync();


            return View(pedidosCliente);
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Carrito)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        public string EstadoPedido(Pedido pedido)
        {
            switch (pedido.Estado)
            {
                case 1: return "Sin Confirmar";
                case 2: return "Confirmado";
                case 3: return "En preparación";
                case 4: return "En reparto";
                case 5: return "Entregado";
                case 6: return "Cancelado";
            }
            return string.Empty;
        }

        private async Task<decimal> calcularEnvio(int clienteId)
        {
            int estadoEntregado = 5;

            var pedidosCliente = await _context.Pedido.Include(p => p.Carrito).Where(p => p.Carrito.ClienteId == clienteId && p.Estado == estadoEntregado).ToListAsync();

            decimal valorAgregado = 80;

            if (pedidosCliente.Count() > 10)
            {
                valorAgregado = 0;
            }
            return valorAgregado;

            if (await ObtenerTemperatura())
                return 120;

            return 80;
        }


        private async Task<bool> ObtenerTemperatura()
        {
            string apiKey = "e77bf38b94ed30b69bddf7c944f0be49";
            string cityName = "Buenos Aires";
            string apiUrl = $"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}";

            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string jsonResult = await response.Content.ReadAsStringAsync();
                Clima climaVm = JsonConvert.DeserializeObject<Clima>(jsonResult);
                double temperatura = climaVm.Main.Temp - Math.Round(273.15);
                string estado = climaVm.Weather.FirstOrDefault().Main;

                return temperatura < 5 || estado.Equals("Rain");
            }

            return false;
        }


        // GET: Pedidos/Create
        [Authorize]
        public async Task<IActionResult> Create() {
            var user = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Email.ToUpper().Equals(user.NormalizedEmail));

            var carritoActivo = await _context.Carrito
                .Include(c => c.Cliente)
                .Include(c => c.CarritoItems)
                .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.ClienteId.Equals(cliente.Id));

            var gastoEnvio = calcularEnvio(cliente.Id);

            if (carritoActivo != null && carritoActivo.CarritoItems.Count > 0) {
                DetallePedidoViewModel viewModel = new DetallePedidoViewModel() {
                    CarritoId = carritoActivo.Id,
                    Productos = carritoActivo.CarritoItems.ToList(),
                    Cliente = cliente.Nombre + " " + cliente.Apellido,
                    Direccion = cliente.Direccion,
                    Subtotal = (decimal)carritoActivo.CarritoItems.Sum(i => i.PrecioUnitarioConDescuento * i.Cantidad),
                    GastoEnvio = await gastoEnvio,
                };

                return View("HacerPedido", viewModel);
            }

            return NotFound();
        }


        // POST: Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Subtotal,GastoEnvio,Total,CarritoId")] DetallePedidoViewModel pedido) {
            if (ModelState.IsValid) {
                Pedido newPedido = new Pedido() {
                    NroPedido = await GenerarNumeroPedido(),
                    CarritoId = pedido.CarritoId,
                    Estado = 1,
                    FechaCompra = DateTime.Now,
                    GastoEnvio = (decimal)pedido.GastoEnvio,
                    Subtotal = pedido.Subtotal,
                    Total = (decimal)pedido.Subtotal + (decimal)pedido.GastoEnvio
                };

                var carrito = await _context.Carrito.FindAsync(pedido.CarritoId);
                carrito.Procesado = true;
                _context.Update(carrito);
                await _context.SaveChangesAsync();

                _context.Add(newPedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", pedido.CarritoId);
            return View("HacerPedido", pedido);
        }


        //cancelar
        public async Task<IActionResult> Cancelar(int? pedidoId)
        {
            var pedido = await _context.Pedido.FindAsync(pedidoId);

            if (pedidoId == null || pedido == null)
            {
                return RedirectToAction(nameof(Index));
            }
            if (pedido.Estado == 1)
            {
                pedido.Estado = 6;
                _context.Update(pedido);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Pedidos/Delete/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Carrito)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pedido == null)
            {
                return Problem("Entity set 'DbContext.Pedido'  is null.");
            }
            var pedido = await _context.Pedido.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedido.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return (_context.Pedido?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<int> GenerarNumeroPedido()
        {
            {
                var pedido = await _context.Pedido.OrderByDescending(c => c.NroPedido).FirstOrDefaultAsync();

                if (pedido == null) { return 30000; }

                return pedido.NroPedido + 5;
            }
        }


    }
}

