using System;
using System.Linq;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class CuentaController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // GET: Cuenta/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Cuenta/Login
        [HttpPost]
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
                    Session["UsuarioId"] = usuario.UsuarioId;
                    Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;
                    Session["Cedula"] = usuario.Cedula;
                    Session["NombreUsuario"] = usuario.NombreUsuario;

                    var usuarioRol = _db.UsuarioRoles
                        .Where(ur => ur.UsuarioId == usuario.UsuarioId)
                        .Select(ur => ur.Rol)
                        .FirstOrDefault();

                    var rol = usuarioRol != null ? usuarioRol.NombreRol : "Cliente";
                    Session["Rol"] = rol;

                    if (rol == "Admin")
                        return RedirectToAction("Dashboard", "Admin");
                    else
                        return RedirectToAction("Dashboard", "Clientes");
                }

                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            }
            return View(model);
        }

        // GET: Cuenta/Registro
        public ActionResult Registro()
        {
            return View();
        }

        // ============================================================
        // GET: Cuenta/SearchActividades?q=term
        // - Si q vacío → devuelve las primeras 200 actividades
        // - Si q tiene texto → filtra por código o nombre
        // ============================================================
        [HttpGet]
        public JsonResult SearchActividades(string q)
        {
            var query = _db.ActividadesEconomicas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a => a.Codigo.Contains(q) || a.Nombre.Contains(q));
            }

            var results = query
                .OrderBy(a => a.Codigo)
                .Take(200)
                .Select(a => new { id = a.Id, text = a.Codigo + " - " + a.Nombre })
                .ToList();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        // ============================================================
        // GET: Cuenta/GetAllActividades
        // Devuelve TODAS las actividades (sin paginar) para cargar
        // el Select2 de una sola vez al abrir el Registro.
        // ============================================================
        [HttpGet]
        public JsonResult GetAllActividades()
        {
            var results = _db.ActividadesEconomicas
                .OrderBy(a => a.Codigo)
                .Select(a => new { id = a.Id, text = a.Codigo + " - " + a.Nombre })
                .ToList();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        // POST: Cuenta/Registro
        [HttpPost]
        public ActionResult Registro(Usuario usuario, string confirmarContraseña, string NombreCompleto,
                                     string Provincia, string Canton, string Distrito)
        {
            // Separar "Nombre completo" en Nombre / Apellidos (formato costarricense: 2 apellidos)
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

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
            {
                ModelState.AddModelError("", "Debe ingresar su nombre completo.");
                ViewBag.NombreCompleto = NombreCompleto;
                return View(usuario);
            }

            if (usuario.Contraseña != confirmarContraseña)
            {
                ModelState.AddModelError("", "Las contraseñas no coinciden.");
                ViewBag.NombreCompleto = NombreCompleto;
                return View(usuario);
            }

            if (_db.Usuarios.Any(u => u.Cedula == usuario.Cedula))
            {
                ModelState.AddModelError("Cedula", "Ya existe un usuario con esta cédula.");
                ViewBag.NombreCompleto = NombreCompleto;
                return View(usuario);
            }

            if (_db.Usuarios.Any(u => u.Correo == usuario.Correo))
            {
                ModelState.AddModelError("Correo", "Ya existe un usuario con este correo.");
                ViewBag.NombreCompleto = NombreCompleto;
                return View(usuario);
            }

            if (!string.IsNullOrWhiteSpace(usuario.NombreUsuario) &&
                _db.Usuarios.Any(u => u.NombreUsuario == usuario.NombreUsuario))
            {
                ModelState.AddModelError("NombreUsuario", "Ya existe un usuario con este nombre de usuario.");
                ViewBag.NombreCompleto = NombreCompleto;
                return View(usuario);
            }

            usuario.Provincia = Provincia;
            usuario.Canton = Canton;
            usuario.Distrito = Distrito;
            usuario.FechaRegistro = DateTime.Now;
            usuario.Activo = true;

            _db.Usuarios.Add(usuario);
            _db.SaveChanges();

            var rolCliente = _db.Roles.FirstOrDefault(r => r.NombreRol == "Cliente");
            if (rolCliente != null)
            {
                var usuarioRol = new UsuarioRol
                {
                    UsuarioId = usuario.UsuarioId,
                    RolId = rolCliente.RolId
                };
                _db.UsuarioRoles.Add(usuarioRol);
                _db.SaveChanges();
            }

            TempData["Mensaje"] = "Cuenta creada exitosamente. Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        // GET: Cuenta/RecuperarClave
        public ActionResult RecuperarClave()
        {
            return View();
        }

        // POST: Cuenta/RecuperarClave
        [HttpPost]
        public ActionResult RecuperarClave(string correo, string nuevaClave, string confirmarClave)
        {
            if (string.IsNullOrEmpty(correo))
            {
                ViewBag.Error = "Debe ingresar un correo electrónico.";
                return View();
            }

            if (string.IsNullOrEmpty(nuevaClave) || nuevaClave.Length < 6)
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

            TempData["Mensaje"] = "Contraseña actualizada exitosamente.";
            return RedirectToAction("Login");
        }

        // GET: Cuenta/CerrarSesion
        public ActionResult CerrarSesion()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            base.Dispose(disposing);
        }
    }

    // ViewModel para Login
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Contraseña { get; set; }
    }
}