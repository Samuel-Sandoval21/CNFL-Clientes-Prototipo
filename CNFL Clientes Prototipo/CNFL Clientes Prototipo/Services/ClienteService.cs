using System.Collections.Generic;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Repositories;

namespace CNFL_Clientes_Prototipo.Services
{
    public class ClienteService
    {
        private readonly ClienteRepository _repository = new ClienteRepository();

        public List<Cliente> ObtenerTodos()
        {
            return _repository.ObtenerTodos();
        }

        public void RegistrarCliente(Cliente cliente)
        {
            // Como Cliente ahora solo tiene: Id, UsuarioId, Direccion, Provincia, Canton, Distrito
            // Validar que tenga los datos mínimos
            if (cliente != null && cliente.UsuarioId > 0)
            {
                _repository.Agregar(cliente);
            }
        }
    }
}