using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class CarritoController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // GET: Carrito
        public ActionResult Index()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var items = _db.CarritoItems
                .Where(c => c.UsuarioId == usuarioId)
                .OrderByDescending(c => c.FechaAgregado)
                .ToList();

            var subtotal = items.Sum(i => i.Precio * i.Cantidad);
            var impuesto = subtotal * 0.13m;
            var total = subtotal + impuesto;

            ViewBag.Subtotal = subtotal;
            ViewBag.Impuesto = impuesto;
            ViewBag.Total = total;

            return View(items);
        }

        // POST: Carrito/Agregar
        [HttpPost]
        public JsonResult Agregar(string productoId, string nombre, string descripcion, decimal precio, string imagen)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false, message = "Debe iniciar sesión." });

            var existente = _db.CarritoItems
                .FirstOrDefault(c => c.UsuarioId == usuarioId && c.ProductoId == productoId);

            if (existente != null)
            {
                existente.Cantidad += 1;
            }
            else
            {
                _db.CarritoItems.Add(new CarritoItem
                {
                    UsuarioId = usuarioId.Value,
                    ProductoId = productoId,
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Precio = precio,
                    Cantidad = 1,
                    Imagen = imagen,
                    FechaAgregado = DateTime.Now
                });
            }
            _db.SaveChanges();

            var totalItems = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).Sum(c => (int?)c.Cantidad) ?? 0;

            return Json(new { success = true, message = "Agregado al carrito", totalItems = totalItems });
        }

        // POST: Carrito/ActualizarCantidad
        [HttpPost]
        public JsonResult ActualizarCantidad(int itemId, int cantidad)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false, message = "Debe iniciar sesión." });

            var item = _db.CarritoItems.FirstOrDefault(c => c.CarritoItemId == itemId && c.UsuarioId == usuarioId);
            if (item == null)
                return Json(new { success = false, message = "Item no encontrado." });

            if (cantidad <= 0)
            {
                _db.CarritoItems.Remove(item);
            }
            else
            {
                item.Cantidad = cantidad;
            }
            _db.SaveChanges();

            var items = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).ToList();
            var subtotal = items.Sum(i => i.Precio * i.Cantidad);
            var impuesto = subtotal * 0.13m;

            return Json(new
            {
                success = true,
                subtotal = subtotal,
                impuesto = impuesto,
                total = subtotal + impuesto,
                totalItems = items.Sum(i => i.Cantidad)
            });
        }

        // POST: Carrito/Eliminar
        [HttpPost]
        public JsonResult Eliminar(int itemId)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false, message = "Debe iniciar sesión." });

            var item = _db.CarritoItems.FirstOrDefault(c => c.CarritoItemId == itemId && c.UsuarioId == usuarioId);
            if (item == null)
                return Json(new { success = false, message = "Item no encontrado." });

            _db.CarritoItems.Remove(item);
            _db.SaveChanges();

            var items = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).ToList();
            var subtotal = items.Sum(i => i.Precio * i.Cantidad);
            var impuesto = subtotal * 0.13m;

            return Json(new
            {
                success = true,
                subtotal = subtotal,
                impuesto = impuesto,
                total = subtotal + impuesto,
                totalItems = items.Sum(i => i.Cantidad)
            });
        }

        // GET: Carrito/Count
        [HttpGet]
        public JsonResult Count()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { totalItems = 0 }, JsonRequestBehavior.AllowGet);

            var totalItems = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).Sum(c => (int?)c.Cantidad) ?? 0;
            return Json(new { totalItems = totalItems }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}