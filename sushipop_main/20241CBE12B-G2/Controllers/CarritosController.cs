using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _20241CBE12B_G2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace _20241CBE12B_G2.Controllers
{
    public class CarritosController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritosController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Carritos
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var dbContext = _context.CarritoItem.Include(c => c.Carrito)
                                                .Include(c => c.Producto)
                                                .Where(c => c.Carrito.Cliente.Email.ToUpper() == user.NormalizedEmail && (c.Carrito.Cancelado == false) && (c.Carrito.Procesado == false));
                                                

            return View(await dbContext.ToListAsync());
        }

        // GET: Carritos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carrito == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // GET: Carritos/Create
        [Authorize(Roles = "CLIENTE")]
        public IActionResult Create()
        {            

            return View();
        }

        [HttpGet("Carritos/Edit/{carritoId}")]
        public async Task<IActionResult> Procesado(int carritoId)
        {
            var carritoBuscado = _context.Carrito.Include(c=> c.Cliente).Where(c => c.Id == carritoId).FirstOrDefault();
            if(carritoBuscado != null)
            {

                Carrito carritoNuevo = new()
                {
                    Cancelado = false,
                    Procesado = false,
                    ClienteId = carritoBuscado.ClienteId
                };

                carritoBuscado.Cliente.Carritos.Add(carritoNuevo);
                _context.Add(carritoNuevo);
                _context.Update(carritoBuscado.Cliente);
                await _context.SaveChangesAsync();

               

                // ↓↓ carritoBuscado pasa a ser un Pedido - desarrollar ↓↓
            }
            return RedirectToAction("Create", "Pedidos");
        }

        [HttpGet("Carritos/Index/{carritoId}")]
        public async Task<IActionResult> Cancelado(int carritoId)
        {
            var carritoBuscado = await _context.Carrito.Include(c => c.Cliente).Include(c=> c.CarritoItems).Where(c => c.Id == carritoId).FirstOrDefaultAsync();
            if (carritoBuscado != null)
            {
                carritoBuscado.Cancelado = true;

                Carrito carritoNuevo = new(){
                    Cancelado = false,
                    Procesado = false,
                    ClienteId= carritoBuscado.ClienteId
                };
                carritoBuscado.Cliente.Carritos.Add(carritoNuevo);

                carritoBuscado.CarritoItems.ForEach(ci =>
                {
                    var productoBuscado = _context.Producto.Where(p => p.Id == ci.ProductoId).FirstOrDefault();
                    if(productoBuscado != null)
                    {
                        productoBuscado.Stock += ci.Cantidad;
                    }
                }
                );
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Carritos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Procesado,Cancelado,ClienteId")] Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carrito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Set<Cliente>(), "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // GET: Carritos/Edit/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carrito == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito.FindAsync(id);
            if (carrito == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Set<Cliente>(), "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // POST: Carritos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Procesado,Cancelado,ClienteId")] Carrito carrito)
        {
            if (id != carrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Set<Cliente>(), "Id", "Apellido", carrito.ClienteId);
            return View(carrito);
        }

        // GET: Carritos/Delete/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carrito == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Carrito == null)
            {
                return Problem("Entity set 'DbContext.Carrito'  is null.");
            }
            var carrito = await _context.Carrito.FindAsync(id);
            if (carrito != null)
            {
                _context.Carrito.Remove(carrito);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoExists(int id)
        {
          return (_context.Carrito?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
