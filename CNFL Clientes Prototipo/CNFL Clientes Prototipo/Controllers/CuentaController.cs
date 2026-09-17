using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class CuentaController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // ═══════════════════════════════════════════════════════════
        // LOGIN (CLIENTE + ADMIN)
        // ═══════════════════════════════════════════════════════════
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string UserName, string Contraseña)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Contraseña))
            {
                ViewBag.Error = "Completá cédula y contraseña.";
                return View();
            }

            var cedulaLimpia = UserName.Trim();

            var usuario = _db.Usuarios.FirstOrDefault(u =>
                (u.Cedula == cedulaLimpia ||
                 u.Correo == cedulaLimpia ||
                 u.NombreUsuario == cedulaLimpia) &&
                u.Contraseña == Contraseña &&
                u.Activo);

            if (usuario == null)
            {
                ViewBag.Error = "Cédula o contraseña incorrectas.";
                return View();
            }

            var esAdmin = _db.UsuarioRoles
                .Any(ur => ur.UsuarioId == usuario.UsuarioId &&
                           ur.Rol.NombreRol == "Admin");

            if (esAdmin)
            {
                Session["AdminLogueado"] = true;
                Session["AdminNombre"] = usuario.Nombre + " " + usuario.Apellidos;
                Session["AdminCorreo"] = usuario.Correo;
                Session["AdminId"] = usuario.UsuarioId;
                Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;
                return RedirectToAction("Dashboard", "Admin");
            }

            // ═══════════════════════════════════════════════════════
            // ES CLIENTE — GUARDAR TODAS LAS CLAVES DE SESIÓN
            // ═══════════════════════════════════════════════════════
            Session["UsuarioId"] = usuario.UsuarioId;
            Session["NombreUsuario"] = usuario.Nombre;
            Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;   // ← FIX
            Session["FotoPerfil"] = usuario.FotoPerfil;

            return RedirectToAction("Dashboard", "Clientes");
        }

        // ═══════════════════════════════════════════════════════════
        // LOGOUT
        // ═══════════════════════════════════════════════════════════
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Cuenta");
        }

        // ═══════════════════════════════════════════════════════════
        // RECUPERAR CLAVE
        // ═══════════════════════════════════════════════════════════
        [AllowAnonymous]
        [HttpGet]
        public ActionResult RecuperarClave()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarClave(string cedula, string nuevaContrasena, string confirmarContrasena)
        {
            if (string.IsNullOrWhiteSpace(cedula) ||
                string.IsNullOrWhiteSpace(nuevaContrasena) ||
                string.IsNullOrWhiteSpace(confirmarContrasena))
            {
                ViewBag.Error = "Completá todos los campos.";
                return View();
            }

            if (nuevaContrasena.Length < 6)
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres.";
                return View();
            }

            if (nuevaContrasena != confirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            var cedulaLimpia = cedula.Trim();

            var usuario = _db.Usuarios.FirstOrDefault(u =>
                u.Cedula == cedulaLimpia || u.Correo == cedulaLimpia);

            if (usuario == null)
            {
                ViewBag.Error = "No existe una cuenta con esa cédula o correo.";
                return View();
            }

            usuario.Contraseña = nuevaContrasena;
            _db.SaveChanges();

            ViewBag.Mensaje = "✅ Contraseña actualizada correctamente. Ya podés iniciar sesión.";
            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // REGISTRO (GET)
        // ═══════════════════════════════════════════════════════════
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Registro()
        {
            CargarListasRegistro();
            return View(new Usuario());
        }

        // ═══════════════════════════════════════════════════════════
        // REGISTRO (POST) — SIN Sexo ni Ubicación
        // ═══════════════════════════════════════════════════════════
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(
            string Cedula,
            string NombreCompleto,
            string NombreUsuario,
            string Correo,
            string Telefono,
            string Contraseña,
            string confirmarContraseña,
            bool? AceptoPolitica,
            bool? AceptoConsentimiento,
            bool? FacturaElectronica,
            int? ActividadEconomicaId)
        {
            if (string.IsNullOrWhiteSpace(Cedula))
                ModelState.AddModelError("Cedula", "La cédula es obligatoria.");

            if (string.IsNullOrWhiteSpace(NombreCompleto))
                ModelState.AddModelError("NombreCompleto", "El nombre completo es obligatorio.");

            if (string.IsNullOrWhiteSpace(NombreUsuario))
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(Correo))
                ModelState.AddModelError("Correo", "El correo es obligatorio.");

            if (string.IsNullOrWhiteSpace(Telefono))
                ModelState.AddModelError("Telefono", "El teléfono es obligatorio.");

            if (string.IsNullOrWhiteSpace(Contraseña) || Contraseña.Length < 6)
                ModelState.AddModelError("Contraseña", "La contraseña debe tener al menos 6 caracteres.");

            if (Contraseña != confirmarContraseña)
                ModelState.AddModelError("confirmarContraseña", "Las contraseñas no coinciden.");

            if (AceptoPolitica != true)
                ModelState.AddModelError("AceptoPolitica", "Debés aceptar la Política de Privacidad.");

            if (AceptoConsentimiento != true)
                ModelState.AddModelError("AceptoConsentimiento", "Debés aceptar el Consentimiento Informado.");

            var cedulaLimpia = (Cedula ?? "").Trim();

            if (_db.Usuarios.Any(u => u.Cedula == cedulaLimpia))
                ModelState.AddModelError("Cedula", "Ya existe una cuenta con esa cédula.");

            if (!string.IsNullOrWhiteSpace(NombreUsuario) &&
                _db.Usuarios.Any(u => u.NombreUsuario == NombreUsuario))
                ModelState.AddModelError("NombreUsuario", "Ese nombre de usuario ya está en uso.");

            if (!string.IsNullOrWhiteSpace(Correo) &&
                _db.Usuarios.Any(u => u.Correo == Correo))
                ModelState.AddModelError("Correo", "Ya existe una cuenta con ese correo.");

            if (FacturaElectronica == true &&
                (!ActividadEconomicaId.HasValue || ActividadEconomicaId.Value <= 0))
                ModelState.AddModelError("ActividadEconomicaId", "Elegí la actividad económica.");

            if (!ModelState.IsValid)
            {
                ViewBag.NombreCompleto = NombreCompleto;
                ViewBag.AceptoPolitica = AceptoPolitica ?? false;
                ViewBag.AceptoConsentimiento = AceptoConsentimiento ?? false;
                ViewBag.FacturaElectronica = FacturaElectronica ?? false;

                CargarListasRegistro();
                return View(new Usuario());
            }

            var partes = (NombreCompleto ?? "").Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            string nombre = partes.Length > 0 ? partes[0] : "";
            string apellidos = partes.Length > 1 ? string.Join(" ", partes.Skip(1)) : "";

            var usuario = new Usuario
            {
                Cedula = cedulaLimpia,
                Nombre = nombre,
                Apellidos = apellidos,
                NombreUsuario = NombreUsuario,
                Correo = Correo,
                Telefono = Telefono,
                Contraseña = Contraseña,
                Activo = true,
                FechaRegistro = DateTime.Now,
                FacturaElectronica = FacturaElectronica ?? false,
                ActividadEconomicaId = (FacturaElectronica == true) ? ActividadEconomicaId : null
            };

            _db.Usuarios.Add(usuario);
            _db.SaveChanges();

            var rolCliente = _db.Roles.FirstOrDefault(r => r.NombreRol == "Cliente");
            if (rolCliente != null)
            {
                _db.UsuarioRoles.Add(new UsuarioRol
                {
                    UsuarioId = usuario.UsuarioId,
                    RolId = rolCliente.RolId
                });
                _db.SaveChanges();
            }

            TempData["MensajeExito"] = "¡Cuenta creada! Ya podés iniciar sesión.";
            return RedirectToAction("Login", "Cuenta");
        }

        // ═══════════════════════════════════════════════════════════
        // COMBOS UBICACIÓN (AJAX)
        // ═══════════════════════════════════════════════════════════
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetCantones(string provincia)
        {
            var lista = new List<string>();
            if (!string.IsNullOrWhiteSpace(provincia) &&
                UbicacionCostaRica.Catalogo.ContainsKey(provincia))
            {
                lista = UbicacionCostaRica.Catalogo[provincia].Keys.ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetDistritos(string provincia, string canton)
        {
            var lista = new List<string>();
            if (!string.IsNullOrWhiteSpace(provincia) &&
                !string.IsNullOrWhiteSpace(canton) &&
                UbicacionCostaRica.Catalogo.ContainsKey(provincia) &&
                UbicacionCostaRica.Catalogo[provincia].ContainsKey(canton))
            {
                lista = UbicacionCostaRica.Catalogo[provincia][canton];
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // ═══════════════════════════════════════════════════════════
        // PARTIALS LEGALES
        // ═══════════════════════════════════════════════════════════
        [AllowAnonymous]
        [HttpGet]
        public ActionResult PoliticaPrivacidad()
        {
            return PartialView("_PoliticaPrivacidad");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult TerminosCondiciones()
        {
            return PartialView("_TerminosCondiciones");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ConsentimientoInformado()
        {
            return PartialView("_ConsentimientoInformado");
        }

        // ═══════════════════════════════════════════════════════════
        // HELPER
        // ═══════════════════════════════════════════════════════════
        private void CargarListasRegistro()
        {
            ViewBag.Provincias = UbicacionCostaRica.Catalogo.Keys.ToList();
            ViewBag.ActividadesEconomicas = _db.ActividadesEconomicas
                .OrderBy(a => a.Codigo)
                .Select(a => new ActividadEconomicaDto
                {
                    Id = a.Id,
                    Codigo = a.Codigo,
                    Nombre = a.Nombre
                })
                .ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}