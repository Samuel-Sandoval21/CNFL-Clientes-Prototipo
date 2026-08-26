using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class CuentaController : Controller
    {
        // Lista estática que simula la base de datos de usuarios
        private static List<Usuario> _usuarios = new List<Usuario>()
        {
            new Usuario { Nombre = "Admin", Apellidos = "Sistema", Cedula = "1-0000-0000", Telefono = "8888-8888", Correo = "admin@cnfl.com", NISE = "0000000-0", Username = "admin", Password = "12345$", Rol = "Admin" },
            new Usuario { Nombre = "María", Apellidos = "González", Cedula = "1-0234-5678", Telefono = "7777-7777", Correo = "maria@cnfl.com", NISE = "1234567-8", Username = "cliente", Password = "123456", Rol = "Cliente" }
        };

        public ActionResult Index()
        {
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
                Session["NISE"] = user.NISE;
                Session["Username"] = user.Username;

                if (user.Rol == "Admin")
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Dashboard", "Clientes");
            }

            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        public ActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registro(string nombre, string apellidos, string cedula, string telefono, string correo, string nise, string usuario)
        {
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(apellidos) && !string.IsNullOrEmpty(cedula) && !string.IsNullOrEmpty(correo) && !string.IsNullOrEmpty(usuario))
            {
                var nuevoUsuario = new Usuario
                {
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Cedula = cedula,
                    Telefono = telefono,
                    Correo = correo,
                    NISE = nise,
                    Username = usuario,
                    Password = "123456",
                    Rol = "Cliente"
                };

                _usuarios.Add(nuevoUsuario);
                return RedirectToAction("Login", "Cuenta");
            }

            ViewBag.Error = "Debe completar todos los campos";
            return View();
        }

        // GET: Ver y Editar Mis Datos
        public ActionResult MisDatos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            var username = Session["Username"]?.ToString();
            var user = _usuarios.Find(u => u.Username == username);

            if (user == null)
                return RedirectToAction("Index", "Cuenta");

            return View(user);
        }

        // POST: Guardar cambios de Mis Datos
        [HttpPost]
        public ActionResult GuardarDatos(string nombre, string apellidos, string cedula, string telefono, string correo)
        {
            var username = Session["Username"]?.ToString();
            var user = _usuarios.Find(u => u.Username == username);

            if (user != null)
            {
                user.Nombre = nombre;
                user.Apellidos = apellidos;
                user.Cedula = cedula;
                user.Telefono = telefono;
                user.Correo = correo;

                // Actualizamos la sesión para que el nombre del menú cambie
                Session["Usuario"] = user.Nombre + " " + user.Apellidos;
                Session["Correo"] = user.Correo;
            }

            return RedirectToAction("MisDatos", "Cuenta");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Cuenta");
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
        public string NISE { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
    }
}