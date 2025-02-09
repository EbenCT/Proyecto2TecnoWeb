using System.Collections.Generic;

namespace MSVenta.Venta.Models
{
    public class Marca
    {
        public int id { get; set; }
        public string nombre { get; set; }

        // Relación con Productos
        public ICollection<Producto> Productos { get; set; }
    }
}