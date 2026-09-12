using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNFL_Clientes_Prototipo.Models
{
    public class CarritoItem
    {
        [Key]
        public int CarritoItemId { get; set; }
        public int UsuarioId { get; set; }
        public string ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string Imagen { get; set; }
        public DateTime FechaAgregado { get; set; }

        [NotMapped]
        public decimal Subtotal => Precio * Cantidad;

        public virtual Usuario Usuario { get; set; }
    }

    public class OrdenCompra
    {
        [Key]
        public int OrdenId { get; set; }
        public string NumeroOrden { get; set; }
        public int UsuarioId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string Metodo { get; set; }
        public string Estado { get; set; }
        public string ReferenciaPago { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public string Detalle { get; set; }

        public virtual Usuario Usuario { get; set; }
    }

    public class MetodoPago
    {
        [Key]
        public int MetodoPagoId { get; set; }
        public int UsuarioId { get; set; }
        public string Tipo { get; set; }
        public string Alias { get; set; }
        public string Ultimos4 { get; set; }
        public string Titular { get; set; }
        public string FechaVencimiento { get; set; }
        public string Banco { get; set; }
        public string CuentaIBAN { get; set; }
        public bool Predeterminado { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual Usuario Usuario { get; set; }
    }

    public class CarritoResumenDto
    {
        public int TotalItems { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>DTO de producto para la tienda</summary>
    public class ProductoTiendaDto
    {
        public string Id { get; set; }
        public string Categoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Icono { get; set; }
    }
}