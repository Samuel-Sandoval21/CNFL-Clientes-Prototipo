using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class CuentaController : Controller
    {
        private static List<Usuario> _usuarios = new List<Usuario>()
        {
            new Usuario { Nombre = "Katherine", Apellidos = "Villalobos", Cedula = "1-2345-6789", Telefono = "8888-7777", Correo = "k.villalobos@correo.cr", Username = "cliente", Password = "123456", Rol = "Cliente", AliasPropiedad = "Casa", NISEs = new List<string> { "NISE 4021", "NISE 7788" } }
        };

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cuenta()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string usuario, string contrasena)
        {
            var user = _usuarios.Find(u => u.Username == usuario && u.Password == contrasena);

            if (user != null)
            {
                Session["Usuario"] = user.Nombre + " " + user.Apellidos;
                Session["Rol"] = user.Rol;
                Session["Correo"] = user.Correo;
                Session["Username"] = user.Username;

                // Redirige al nuevo Home del prototipo
                return RedirectToAction("Index", "Cuenta");
            }

            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        public ActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registro(string nombre, string apellidos, string cedula, string telefono, string correo, string alias)
        {
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(cedula) && !string.IsNullOrEmpty(correo))
            {
                var nuevoUsuario = new Usuario
                {
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Cedula = cedula,
                    Telefono = telefono,
                    Correo = correo,
                    Username = nombre.ToLower().Replace(" ", ""),
                    Password = "123456",
                    Rol = "Cliente",
                    AliasPropiedad = alias
                };

                _usuarios.Add(nuevoUsuario);
                return RedirectToAction("Login", "Cuenta");
            }

            ViewBag.Error = "Debe completar todos los campos";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Cuenta");
        }

        public static List<Usuario> ObtenerUsuarios()
        {
            return _usuarios;
        }
    }

    public class Usuario
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        public string AliasPropiedad { get; set; }
        public List<string> NISEs { get; set; } = new List<string>();
    }
}