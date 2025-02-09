using System.Collections.Generic;
using System;

namespace MSVenta.Venta.Models
{
    public class AjusteInventario
    {
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
        public int tipo { get; set; } //  (0 = Egreso, 1 = Ingreso)
        public int usuarioId { get; set; }
        public ICollection<DetalleAjuste> DetallesAjuste { get; set; }
    }

}