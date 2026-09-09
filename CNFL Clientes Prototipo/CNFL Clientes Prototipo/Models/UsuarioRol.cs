namespace CNFL_Clientes_Prototipo.Models
{
    public class UsuarioRol
    {
        public int UsuarioRolId { get; set; }
        public int UsuarioId { get; set; }
        public int RolId { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual Rol Rol { get; set; }
    }
}