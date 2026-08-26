using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Repositories
{
    public class ClienteRepository
    {
        // Lista estática para simular la base de datos
        private static List<Cliente> _clientes = new List<Cliente>()
        {
            new Cliente { Id = 1, Nombre = "Samuel", Apellidos = "Sandoval", Identificacion = "123456789", Telefono = "8888-8888", Correo = "samuel@cnfl.com" },
            new Cliente { Id = 2, Nombre = "Ana", Apellidos = "Rodríguez", Identificacion = "987654321", Telefono = "7777-7777", Correo = "ana@cnfl.com" }
        };

        public List<Cliente> ObtenerTodos()
        {
            return _clientes;
        }

        public void Agregar(Cliente cliente)
        {
            cliente.Id = _clientes.Max(c => c.Id) + 1;
            _clientes.Add(cliente);
        }
    }
}