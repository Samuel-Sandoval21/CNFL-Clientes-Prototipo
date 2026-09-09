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
                    (u.Cedula == model.UserName || u.Correo == model.UserName) &&
                    u.Contraseña == model.Contraseña);

                if (usuario != null && usuario.Activo)
                {
                    Session["UsuarioId"] = usuario.UsuarioId;
                    Session["Nombre"] = usuario.Nombre + " " + usuario.Apellidos;
                    Session["Cedula"] = usuario.Cedula;

                    var usuarioRol = _db.UsuarioRoles
                        .Where(ur => ur.UsuarioId == usuario.UsuarioId)
                        .Select(ur => ur.Rol)
                        .FirstOrDefault();

                    var rol = usuarioRol != null ? usuarioRol.NombreRol : "Cliente";
                    Session["Rol"] = rol;

                    if (rol == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Clientes");
                    }
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

        // GET: Cuenta/SearchActividades?q=term
        [HttpGet]
        public JsonResult SearchActividades(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(new object[0], JsonRequestBehavior.AllowGet);

            var results = _db.ActividadesEconomicas
                .Where(a => a.Codigo.Contains(q) || a.Nombre.Contains(q))
                .Select(a => new { id = a.Id, text = a.Codigo + " - " + a.Nombre })
                .Take(20)
                .ToList();

            return Json(results, JsonRequestBehavior.AllowGet);
        }

        // POST: Cuenta/Registro
        [HttpPost]
        public ActionResult Registro(Usuario usuario, string confirmarContraseña)
        {
            if (ModelState.IsValid)
            {
                if (usuario.Contraseña != confirmarContraseña)
                {
                    ModelState.AddModelError("", "Las contraseñas no coinciden.");
                    return View(usuario);
                }

                if (_db.Usuarios.Any(u => u.Cedula == usuario.Cedula))
                {
                    ModelState.AddModelError("Cedula", "Ya existe un usuario con esta cédula.");
                    return View(usuario);
                }

                if (_db.Usuarios.Any(u => u.Correo == usuario.Correo))
                {
                    ModelState.AddModelError("Correo", "Ya existe un usuario con este correo.");
                    return View(usuario);
                }

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
            return View(usuario);
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