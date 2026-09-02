using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Repositories
{
    public class ClienteRepository
    {
        private static List<Cliente> _clientes = new List<Cliente>();

        public List<Cliente> ObtenerTodos()
        {
            return _clientes;
        }

        public Cliente ObtenerPorId(int id)
        {
            return _clientes.FirstOrDefault(c => c.Id == id);
        }

        public Cliente ObtenerPorUsuarioId(int usuarioId)
        {
            return _clientes.FirstOrDefault(c => c.UsuarioId == usuarioId);
        }

        public void Agregar(Cliente cliente)
        {
            cliente.Id = _clientes.Count > 0 ? _clientes.Max(c => c.Id) + 1 : 1;
            _clientes.Add(cliente);
        }

        public void Actualizar(Cliente cliente)
        {
            var index = _clientes.FindIndex(c => c.Id == cliente.Id);
            if (index != -1)
            {
                _clientes[index] = cliente;
            }
        }

        public void Eliminar(int id)
        {
            var cliente = _clientes.FirstOrDefault(c => c.Id == id);
            if (cliente != null)
            {
                _clientes.Remove(cliente);
            }
        }
    }
}