using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Notificacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; } // "Factura", "Avería", "Corte", "Actualización"
        public string Fecha { get; set; }
        public bool Leida { get; set; }
    }
}