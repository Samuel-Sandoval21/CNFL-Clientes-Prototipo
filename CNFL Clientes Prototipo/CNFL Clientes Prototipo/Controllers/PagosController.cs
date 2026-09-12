using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class PagosController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // GET: Pagos/Checkout
        public ActionResult Checkout()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var items = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).ToList();
            if (items.Count == 0)
                return RedirectToAction("Index", "Carrito");

            var subtotal = items.Sum(i => i.Precio * i.Cantidad);
            var impuesto = subtotal * 0.13m;

            ViewBag.Subtotal = subtotal;
            ViewBag.Impuesto = impuesto;
            ViewBag.Total = subtotal + impuesto;
            ViewBag.Items = items;
            ViewBag.MetodosGuardados = _db.MetodosPago
                .Where(m => m.UsuarioId == usuarioId && m.Activo)
                .OrderByDescending(m => m.Predeterminado)
                .ToList();

            return View();
        }

        // GET: Pagos/Tarjeta
        public ActionResult Tarjeta()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            var items = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).ToList();
            if (items.Count == 0) return RedirectToAction("Index", "Carrito");

            var subtotal = items.Sum(i => i.Precio * i.Cantidad);
            ViewBag.Subtotal = subtotal;
            ViewBag.Impuesto = subtotal * 0.13m;
            ViewBag.Total = subtotal + (subtotal * 0.13m);
            return View();
        }

        // GET: Pagos/Sinpe
        public ActionResult Sinpe()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            var items = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).ToList();
            if (items.Count == 0) return RedirectToAction("Index", "Carrito");

            var subtotal = items.Sum(i => i.Precio * i.Cantidad);
            ViewBag.Subtotal = subtotal;
            ViewBag.Impuesto = subtotal * 0.13m;
            ViewBag.Total = subtotal + (subtotal * 0.13m);
            ViewBag.TelefonoSinpe = "8888-1234";
            return View();
        }

        // GET: Pagos/Iban
        public ActionResult Iban()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            var items = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).ToList();
            if (items.Count == 0) return RedirectToAction("Index", "Carrito");

            var subtotal = items.Sum(i => i.Precio * i.Cantidad);
            ViewBag.Subtotal = subtotal;
            ViewBag.Impuesto = subtotal * 0.13m;
            ViewBag.Total = subtotal + (subtotal * 0.13m);
            ViewBag.CuentaIBAN = "CR05015200100512345678";
            ViewBag.Banco = "Banco Nacional de Costa Rica";
            ViewBag.Titular = "Compañía Nacional de Fuerza y Luz S.A.";
            return View();
        }

        // GET: Pagos/Tokens
        public ActionResult Tokens()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");
            var items = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).ToList();
            if (items.Count == 0) return RedirectToAction("Index", "Carrito");

            var subtotal = items.Sum(i => i.Precio * i.Cantidad);
            ViewBag.Subtotal = subtotal;
            ViewBag.Impuesto = subtotal * 0.13m;
            ViewBag.Total = subtotal + (subtotal * 0.13m);
            ViewBag.TokensDisponibles = 150;
            return View();
        }

        // POST: Pagos/Procesar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Procesar(string metodo, string referencia)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false, message = "Debe iniciar sesión." });

            var items = _db.CarritoItems.Where(c => c.UsuarioId == usuarioId).ToList();
            if (items.Count == 0)
                return Json(new { success = false, message = "El carrito está vacío." });

            var subtotal = items.Sum(i => i.Precio * i.Cantidad);
            var impuesto = subtotal * 0.13m;
            var total = subtotal + impuesto;

            var anio = DateTime.Now.Year;
            var count = _db.OrdenesCompra.Count() + 1;
            var numeroOrden = $"ORD-{anio}-{count:D5}";

            var orden = new OrdenCompra
            {
                NumeroOrden = numeroOrden,
                UsuarioId = usuarioId.Value,
                Subtotal = subtotal,
                Impuesto = impuesto,
                Total = total,
                Metodo = metodo,
                Estado = "Confirmado",
                ReferenciaPago = referencia ?? numeroOrden,
                FechaCreacion = DateTime.Now,
                FechaConfirmacion = DateTime.Now,
                Detalle = JsonConvert.SerializeObject(items.Select(i => new
                {
                    i.ProductoId,
                    i.Nombre,
                    i.Cantidad,
                    i.Precio,
                    Subtotal = i.Precio * i.Cantidad
                }))
            };

            _db.OrdenesCompra.Add(orden);

            _db.Notificaciones.Add(new Notificacion
            {
                UsuarioId = usuarioId.Value,
                Titulo = "Pago realizado",
                Mensaje = $"Su orden {numeroOrden} por ₡{total:N0} fue confirmada mediante {metodo}.",
                Fecha = DateTime.Now,
                Leida = false,
                Tipo = "Pago"
            });

            _db.CarritoItems.RemoveRange(items);
            _db.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Pago procesado exitosamente.",
                ordenId = orden.OrdenId,
                numeroOrden = numeroOrden
            });
        }

        // GET: Pagos/Confirmacion/{id}
        public ActionResult Confirmacion(int id)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var orden = _db.OrdenesCompra.FirstOrDefault(o => o.OrdenId == id && o.UsuarioId == usuarioId);
            if (orden == null) return HttpNotFound();
            return View(orden);
        }

        // GET: Pagos/MisCompras
        public ActionResult MisCompras()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Login", "Cuenta");

            var ordenes = _db.OrdenesCompra
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.FechaCreacion)
                .ToList();
            return View(ordenes);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}