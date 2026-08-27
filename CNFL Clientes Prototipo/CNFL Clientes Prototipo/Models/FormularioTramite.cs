using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CNFL_Clientes_Prototipo.Models
{
    public class FormularioTramite
    {
        public int Id { get; set; }
        public string NombreTramite { get; set; }
        public string NISE { get; set; }
        public string NombreCliente { get; set; }
        public string Cedula { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Detalle { get; set; }
    }
}