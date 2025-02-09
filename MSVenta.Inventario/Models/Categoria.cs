using System.Collections.Generic;

namespace MSVenta.Venta.Models
{
    public class Categoria
    {
        public int id { get; set; }
        public string nombre { get; set; }

        // Relación con Productos
        public ICollection<Producto> Productos { get; set; }
    }
}
