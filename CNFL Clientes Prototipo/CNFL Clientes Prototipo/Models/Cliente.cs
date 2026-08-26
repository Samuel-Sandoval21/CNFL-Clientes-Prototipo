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
        public string Apellidos { get; set; }
        public string Identificacion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
    }
}