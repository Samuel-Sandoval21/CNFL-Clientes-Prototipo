using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Repositories;

namespace CNFL_Clientes_Prototipo.Services
{
    public class ClienteService
    {
        private readonly ClienteRepository _repository = new ClienteRepository();

        public List<Cliente> ListarClientes()
        {
            return _repository.ObtenerTodos();
        }

        public void RegistrarCliente(Cliente cliente)
        {
            // Validación simple del prototipo
            if (!string.IsNullOrEmpty(cliente.Nombre) && !string.IsNullOrEmpty(cliente.Identificacion))
            {
                _repository.Agregar(cliente);
            }
        }
    }
}