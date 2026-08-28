using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace CNFL_Clientes_Prototipo.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }      // <-- Agregar
        public string Identificacion { get; set; } // <-- Agregar (cédula)
        public string Cedula { get; set; }
        public string NISE { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool Activo { get; set; } = true;
    }
}