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
        // LOGIN
        // ═══════════════════════════════════════════════════════════

        public ActionResult Login()
        {
            if (Session["UsuarioId"] != null)
                return RedirectToAction("Dashboard", "Clientes");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = _db.Usuarios.FirstOrDefault(u =>
                    (u.Cedula == model.UserName
                     || u.Correo == model.UserName
                     || u.NombreUsuario == model.UserName)
                    && u.Contraseña == model.Contraseña);

                if (usuario != null && usuario.Activo)
                {
                    CrearSesion(usuario);

                    var rol = Session["Rol"] as string ?? "Cliente";
                    return rol == "Admin"
                        ? RedirectToAction("Dashboard", "Admin")
                        : RedirectToAction("Dashboard", "Clientes");
                }

                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            }
            return View(model);
        }

        // ═══════════════════════════════════════════════════════════
        // LOGIN BIOMÉTRICO (Face ID / Huella)
        // ═══════════════════════════════════════════════════════════

        [HttpPost]
        public JsonResult LoginBiometrico(int usuarioId, string token, string tipo)
        {
            try
            {
                var usuario = _db.Usuarios.Find(usuarioId);
                if (usuario == null || !usuario.Activo)
                    return Json(new { success = false, message = "Usuario no encontrado o inactivo." });

                var tokenEsperado = GenerarTokenBiometrico(usuario.Cedula);
                if (token != tokenEsperado)
                    return Json(new { success = false, message = "Verificación biométrica inválida." });

                CrearSesion(usuario);
                Session["Biometrico"] = tipo;

                var rol = Session["Rol"] as string ?? "Cliente";
                var redirectUrl = rol == "Admin"
                    ? Url.Action("Dashboard", "Admin")
                    : Url.Action("Dashboard", "Clientes");

                try
                {
                    _db.ActividadUsuario.Add(new ActividadUsuario
                    {
                        UsuarioId = usuario.UsuarioId,
                        Seccion = "Login",
                        Accion = "Biometrico",
                        Detalle = "Login con " + tipo,
                        Fecha = DateTime.Now
                    });
                    _db.SaveChanges();
                }
                catch { /* silencioso */ }

                return Json(new { success = true, redirectUrl = redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetUsuarioPorCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            var usuario = _db.Usuarios.FirstOrDefault(u => u.Cedula == cedula && u.Activo);
            if (usuario == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                success = true,
                usuarioId = usuario.UsuarioId,
                token = GenerarTokenBiometrico(usuario.Cedula)
            }, JsonRequestBehavior.AllowGet);
        }

        private string GenerarTokenBiometrico(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula)) return "";
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(cedula + "CNFL_BIOMETRICO_2026");
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash).Substring(0, 24);
            }
        }

        private void CrearSesion(Usuario usuario)
        {
            Session["UsuarioId"] = usuario.UsuarioId;
            Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;
            Session["Cedula"] = usuario.Cedula;
            Session["NombreUsuario"] = usuario.NombreUsuario;

            var usuarioRol = _db.UsuarioRoles
                .Where(ur => ur.UsuarioId == usuario.UsuarioId)
                .Select(ur => ur.Rol)
                .FirstOrDefault();

            Session["Rol"] = usuarioRol != null ? usuarioRol.NombreRol : "Cliente";
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

        public ActionResult CerrarSesion()
        {
            return Logout();
        }

        // ═══════════════════════════════════════════════════════════
        // REGISTRO
        // ═══════════════════════════════════════════════════════════

        public ActionResult Registro()
        {
            CargarListasRegistro();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registro(
            Usuario usuario,
            string confirmarContraseña,
            string NombreCompleto,
            string DireccionExacta,
            string Provincia,
            string Canton,
            string Distrito,
            bool AceptoPolitica = false,
            bool AceptoConsentimiento = false,
            bool FacturaElectronica = false,
            bool RegistrarBiometrico = false,
            int? ActividadEconomicaId = null)
        {
            // Separar "Nombre completo" en Nombre / Apellidos
            if (!string.IsNullOrWhiteSpace(NombreCompleto))
            {
                var partes = NombreCompleto.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length >= 3)
                {
                    usuario.Apellidos = string.Join(" ", partes.Skip(partes.Length - 2));
                    usuario.Nombre = string.Join(" ", partes.Take(partes.Length - 2));
                }
                else if (partes.Length == 2)
                {
                    usuario.Nombre = partes[0];
                    usuario.Apellidos = partes[1];
                }
                else
                {
                    usuario.Nombre = NombreCompleto.Trim();
                    usuario.Apellidos = "";
                }
            }

            // Validaciones
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                ModelState.AddModelError("", "Debe ingresar su nombre completo.");

            if (string.IsNullOrWhiteSpace(usuario.Cedula))
                ModelState.AddModelError("", "Debe ingresar su cédula.");

            if (string.IsNullOrWhiteSpace(usuario.Correo))
                ModelState.AddModelError("", "Debe ingresar su correo principal.");

            if (string.IsNullOrWhiteSpace(usuario.Telefono))
                ModelState.AddModelError("", "Debe ingresar su teléfono principal.");

            if (string.IsNullOrWhiteSpace(DireccionExacta))
                ModelState.AddModelError("", "Debe ingresar su dirección exacta.");

            if (string.IsNullOrWhiteSpace(Provincia))
                ModelState.AddModelError("", "Debe seleccionar la provincia.");

            if (string.IsNullOrWhiteSpace(Canton))
                ModelState.AddModelError("", "Debe seleccionar el cantón.");

            if (string.IsNullOrWhiteSpace(Distrito))
                ModelState.AddModelError("", "Debe seleccionar el distrito.");

            if (string.IsNullOrWhiteSpace(usuario.Contraseña) || usuario.Contraseña.Length < 6)
                ModelState.AddModelError("", "La contraseña debe tener al menos 6 caracteres.");

            if (usuario.Contraseña != confirmarContraseña)
                ModelState.AddModelError("", "Las contraseñas no coinciden.");

            if (!AceptoPolitica)
                ModelState.AddModelError("", "Debe aceptar la Política de Privacidad.");

            if (!AceptoConsentimiento)
                ModelState.AddModelError("", "Debe aceptar el Consentimiento Informado.");

            if (!string.IsNullOrWhiteSpace(usuario.Cedula) &&
                _db.Usuarios.Any(u => u.Cedula == usuario.Cedula))
                ModelState.AddModelError("", "Ya existe un usuario con esta cédula.");

            if (!string.IsNullOrWhiteSpace(usuario.Correo) &&
                _db.Usuarios.Any(u => u.Correo == usuario.Correo))
                ModelState.AddModelError("", "Ya existe un usuario con este correo.");

            // ✅ NUEVO: Validar nombre de usuario único
            if (!string.IsNullOrWhiteSpace(usuario.NombreUsuario) &&
                _db.Usuarios.Any(u => u.NombreUsuario == usuario.NombreUsuario))
                ModelState.AddModelError("", "Ya existe un usuario con ese nombre de usuario.");

            if (FacturaElectronica && !ActividadEconomicaId.HasValue)
                ModelState.AddModelError("", "Elegí la actividad económica para activar la factura electrónica.");

            if (!ModelState.IsValid)
            {
                ViewBag.NombreCompleto = NombreCompleto;
                ViewBag.DireccionExacta = DireccionExacta;
                ViewBag.Provincia = Provincia;
                ViewBag.Canton = Canton;
                ViewBag.Distrito = Distrito;
                ViewBag.AceptoPolitica = AceptoPolitica;
                ViewBag.AceptoConsentimiento = AceptoConsentimiento;
                ViewBag.FacturaElectronica = FacturaElectronica;
                CargarListasRegistro();
                return View(usuario);
            }

            usuario.Provincia = Provincia;
            usuario.Canton = Canton;
            usuario.Distrito = Distrito;
            usuario.DireccionExacta = DireccionExacta;
            usuario.FechaRegistro = DateTime.Now;
            usuario.Activo = true;

            usuario.FacturaElectronica = FacturaElectronica;
            usuario.ActividadEconomicaId = FacturaElectronica ? ActividadEconomicaId : null;

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

            TempData["Mensaje"] = "Cuenta creada correctamente. Ya podés iniciar sesión.";

            if (RegistrarBiometrico)
            {
                return RedirectToAction("Login", new { bio = usuario.Cedula });
            }

            return RedirectToAction("Login");
        }

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

        // ═══════════════════════════════════════════════════════════
        // AJAX: Provincia → Cantón → Distrito
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public JsonResult GetCantones(string provincia)
        {
            if (string.IsNullOrWhiteSpace(provincia) ||
                !UbicacionCostaRica.Catalogo.ContainsKey(provincia))
                return Json(new string[0], JsonRequestBehavior.AllowGet);

            var cantones = UbicacionCostaRica.Catalogo[provincia].Keys.ToList();
            return Json(cantones, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDistritos(string provincia, string canton)
        {
            if (string.IsNullOrWhiteSpace(provincia) ||
                string.IsNullOrWhiteSpace(canton) ||
                !UbicacionCostaRica.Catalogo.ContainsKey(provincia) ||
                !UbicacionCostaRica.Catalogo[provincia].ContainsKey(canton))
                return Json(new string[0], JsonRequestBehavior.AllowGet);

            var distritos = UbicacionCostaRica.Catalogo[provincia][canton];
            return Json(distritos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SearchActividades(string q)
        {
            var query = _db.ActividadesEconomicas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(a => a.Codigo.Contains(q) || a.Nombre.Contains(q));
            }

            var results = query
                .OrderBy(a => a.Codigo)
                .Take(50)
                .Select(a => new { id = a.Id, text = a.Codigo + " - " + a.Nombre })
                .ToList();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        // ═══════════════════════════════════════════════════════════
        // RECUPERAR CLAVE
        // ═══════════════════════════════════════════════════════════

        public ActionResult RecuperarClave()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarClave(string correo, string nuevaClave, string confirmarClave)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                ViewBag.Error = "Ingresá tu correo electrónico.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(nuevaClave) || nuevaClave.Length < 6)
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres.";
                return View();
            }

            if (nuevaClave != confirmarClave)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            var usuario = _db.Usuarios.FirstOrDefault(u => u.Correo == correo);
            if (usuario == null)
            {
                ViewBag.Error = "No existe una cuenta con ese correo electrónico.";
                return View();
            }

            usuario.Contraseña = nuevaClave;
            _db.SaveChanges();

            TempData["Mensaje"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Contraseña { get; set; }
    }
}