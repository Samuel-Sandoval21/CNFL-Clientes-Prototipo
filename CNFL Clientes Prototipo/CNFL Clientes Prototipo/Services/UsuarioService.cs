using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Services
{
    public class UsuarioService
    {
        private CNFLDbContext _db = new CNFLDbContext();

        public Usuario ObtenerPorId(int id)
        {
            return _db.Usuarios.Find(id);
        }

        public Usuario ObtenerPorCedula(string cedula)
        {
            return _db.Usuarios.FirstOrDefault(u => u.Cedula == cedula);
        }

        public Usuario ObtenerPorCorreo(string correo)
        {
            return _db.Usuarios.FirstOrDefault(u => u.Correo == correo);
        }

        public List<Usuario> ObtenerTodos()
        {
            return _db.Usuarios.ToList();
        }

        public List<Usuario> ObtenerClientes()
        {
            return _db.Usuarios
                .Where(u => u.UsuarioRoles.Any(ur => ur.Rol.NombreRol == "Cliente"))
                .ToList();
        }

        public bool ValidarLogin(string usuario, string contraseña)
        {
            return _db.Usuarios.Any(u =>
                (u.Cedula == usuario || u.Correo == usuario) &&
                u.Contraseña == contraseña &&
                u.Activo);
        }

        public Usuario Login(string usuario, string contraseña)
        {
            return _db.Usuarios.FirstOrDefault(u =>
                (u.Cedula == usuario || u.Correo == usuario) &&
                u.Contraseña == contraseña &&
                u.Activo);
        }

        public void Registrar(Usuario usuario)
        {
            usuario.FechaRegistro = DateTime.Now;
            usuario.Activo = true;
            _db.Usuarios.Add(usuario);
            _db.SaveChanges();

            // Asignar rol Cliente por defecto
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
        }

        public bool ExisteCedula(string cedula)
        {
            return _db.Usuarios.Any(u => u.Cedula == cedula);
        }

        public bool ExisteCorreo(string correo)
        {
            return _db.Usuarios.Any(u => u.Correo == correo);
        }

        public void Actualizar(Usuario usuario)
        {
            var existente = _db.Usuarios.Find(usuario.UsuarioId);
            if (existente != null)
            {
                existente.Nombre = usuario.Nombre;
                existente.Apellidos = usuario.Apellidos;
                existente.Correo = usuario.Correo;
                existente.Telefono = usuario.Telefono;
                _db.SaveChanges();
            }
        }

        public void CambiarContraseña(int usuarioId, string nuevaContraseña)
        {
            var usuario = _db.Usuarios.Find(usuarioId);
            if (usuario != null)
            {
                usuario.Contraseña = nuevaContraseña;
                _db.SaveChanges();
            }
        }

        public bool EsAdmin(int usuarioId)
        {
            return _db.UsuarioRoles.Any(ur =>
                ur.UsuarioId == usuarioId &&
                ur.Rol.NombreRol == "Admin");
        }

        public string ObtenerNombreCompleto(int usuarioId)
        {
            var usuario = _db.Usuarios.Find(usuarioId);
            return usuario != null ? $"{usuario.Nombre} {usuario.Apellidos}" : "";
        }
    }
}