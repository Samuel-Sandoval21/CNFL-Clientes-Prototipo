using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}