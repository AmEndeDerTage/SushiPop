﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _20241CBE12B_G2.Models;
using Microsoft.AspNetCore.Authorization;

namespace _20241CBE12B_G2.Controllers
{
    public class ContactosController : Controller
    {
        private readonly DbContext _context;

        public ContactosController(DbContext context)
        {
            _context = context;
        }

        // GET: Contactos
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> Index()
        {
              return _context.Contacto != null ? 
                          View(await _context.Contacto.ToListAsync()) :
                          Problem("Entity set 'DbContext.Contacto'  is null.");
        }

        // GET: Contactos/Details/5
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contacto == null)
            {
                return NotFound();
            }

            var contacto = await _context.Contacto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contacto == null)
            {
                return NotFound();
            }

            return View(contacto);
        }

        // GET: Contactos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contactos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,NombreCompleto,Email,Telefono,Mensaje,Leido")] Contacto contacto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contacto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contacto);
        }

        // GET: Contactos/Edit/5
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contacto == null)
            {
                return NotFound();
            }

            var contacto = await _context.Contacto.FindAsync(id);
            if (contacto == null)
            {
                return NotFound();
            }
            return View(contacto);
        }

        // POST: Contactos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreCompleto,Email,Telefono,Mensaje,Leido")] Contacto contacto)
        {
            if (id != contacto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contacto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactoExists(contacto.Id))
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
            return View(contacto);
        }

        // GET: Contactos/Delete/5
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacto == null)
            {
                return NotFound();
            }

            var contacto = await _context.Contacto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contacto == null)
            {
                return NotFound();
            }

            return View(contacto);
        }

        // POST: Contactos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO, ADMIN")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contacto == null)
            {
                return Problem("Entity set 'DbContext.Contacto'  is null.");
            }
            var contacto = await _context.Contacto.FindAsync(id);
            if (contacto != null)
            {
                _context.Contacto.Remove(contacto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactoExists(int id)
        {
          return (_context.Contacto?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
