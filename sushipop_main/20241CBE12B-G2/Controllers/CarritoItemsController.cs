using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _20241CBE12B_G2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace _20241CBE12B_G2.Controllers
{
    public class CarritoItemsController : Controller
    {
        private readonly DbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritoItemsController(DbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: CarritoItems
        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var dbContext = _context.CarritoItem.Include(c => c.Carrito)
                                                .Include(c => c.Producto)
                                                .Where(c => c.Carrito.Cliente.Email.ToUpper() == user.NormalizedEmail)
                                                .ToListAsync();



            return View(await dbContext);
        }

        // GET: CarritoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // GET: CarritoItems/Create
        [Authorize(Roles = "CLIENTE")]
        [HttpGet("Carrito/Create/{productoId?}")]
        public async Task<IActionResult> Create(int? productoId)
        {
            if (productoId == null) { }

            var producto = await _context.Producto.FindAsync(productoId);

            if(producto == null) { }

            var user = await _userManager.GetUserAsync(User);

            if (user == null) { }

            //Si usuario no es nulo
            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

            var carritoCliente = await _context.Carrito
                                    .Include(Carrito => Carrito.Cliente)
                                    .Include(Carrito => Carrito.CarritoItems)
                                    .Where(Carrito =>
                                    Carrito.Cliente.Email.ToUpper() == user.NormalizedEmail 
                                    &&
                                    Carrito.Cancelado == false 
                                    && 
                                    Carrito.Procesado == false)
                                    .FirstOrDefaultAsync();

            if (carritoCliente == null)
            {
                //Carrito nulo, entonces lo creo
                Carrito carritoNuevo = new Carrito();
                carritoNuevo.Procesado = false;
                carritoNuevo.Cancelado = false;
                carritoNuevo.ClienteId = cliente.Id;
                _context.Add(carritoNuevo);
                await _context.SaveChangesAsync();

                carritoCliente = await _context.Carrito
                    .Include(x=>x.Cliente)
                    .Include(x=>x.CarritoItems)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync();
            }

            var precioProducto = producto.Precio;

            //sacar dia
            int dia = 1;
            var descuento = await _context.Descuento
                .Where(descuento => descuento.ProductoId == producto.Id 
                && 
                descuento.Activo == true
                &&
                descuento.Dia == dia)
                .FirstOrDefaultAsync();

            if(descuento != null)
            {
                precioProducto = precioProducto * (1 - descuento.Porcentaje / 100);
            }

            var itemBuscado = await _context.CarritoItem
                .Where(CarritoItem => CarritoItem.CarritoId == carritoCliente.Id
                &&
                CarritoItem.ProductoId == producto.Id)
                .FirstOrDefaultAsync();

            if (itemBuscado == null)
            {
                CarritoItem carritoItem = new CarritoItem();
                carritoItem.PrecioUnitarioConDescuento = precioProducto;
                carritoItem.Cantidad = 0;
                carritoItem.CarritoId = carritoCliente.Id;
                carritoItem.Producto = producto;
                carritoItem.ProductoId = producto.Id;

                itemBuscado = carritoItem;

                if(itemBuscado.Producto.Stock > 0) { 
                    _context.Add(carritoItem);
                    await _context.SaveChangesAsync();
                }
            }

                int stockProducto = producto.Stock;
                int cantidadItemsCarrito = itemBuscado.Cantidad;

                if(stockProducto < cantidadItemsCarrito)
                {
                    //Mensaje de que no hay stock
                }

                if(stockProducto > 0) { 

                itemBuscado.Cantidad += 1;
                producto.Stock -= 1;

                _context.Update(itemBuscado);
                await _context.SaveChangesAsync();
            }

            //ir a la grilla de items
            return RedirectToAction("", "Productos");
        }

        // POST: CarritoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> Create([Bind("Id,PrecioUnitarioConDescuento,Cantidad,CarritoId,ProductoId")] CarritoItem carritoItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carritoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Descripcion", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // GET: CarritoItems/Edit/5
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem.FindAsync(id);
            if (carritoItem == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Descripcion", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // POST: CarritoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrecioUnitarioConDescuento,Cantidad,CarritoId,ProductoId")] CarritoItem carritoItem)
        {
            if (id != carritoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carritoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoItemExists(carritoItem.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Descripcion", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // GET: CarritoItems/Delete/5
        [Authorize(Roles = "CLIENTE")]
        [HttpGet("CarritoItems/Delete/{carritoItemId}")]
        public async Task<IActionResult> Delete(int? carritoItemId)
        {
            if (carritoItemId == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == carritoItemId);
            if (carritoItem == null)
            {
                return NotFound();
            }

            var cantidad = carritoItem.Cantidad;
            var producto = carritoItem.Producto;

            producto.Stock += cantidad;

            _context.Remove(carritoItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("", "Carritos");
        }

        // POST: CarritoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CarritoItem == null)
            {
                return Problem("Entity set 'DbContext.CarritoItem'  is null.");
            }
            var carritoItem = await _context.CarritoItem.FindAsync(id);
            if (carritoItem != null)
            {
                _context.CarritoItem.Remove(carritoItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoItemExists(int id)
        {
          return (_context.CarritoItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
