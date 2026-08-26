using System.Collections.Generic;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Repositories;

namespace CNFL_Clientes_Prototipo.Services
{
    public class AveriaService
    {
        private readonly AveriaRepository _repository = new AveriaRepository();

        public List<Averia> ListarAverias()
        {
            return _repository.ObtenerTodas();
        }

        public void RegistrarAveria(Averia averia)
        {
            _repository.Agregar(averia);
        }

        public void CambiarEstado(int id, string nuevoEstado)
        {
            _repository.ActualizarEstado(id, nuevoEstado);
        }
    }
}