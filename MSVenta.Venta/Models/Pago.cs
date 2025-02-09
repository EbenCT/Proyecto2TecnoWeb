using System;

namespace MSVenta.Venta.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public Venta Venta { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
        public int? MetodoPagoId { get; set; }
        public MetodoPago MetodoPago { get; set; }
    }
}