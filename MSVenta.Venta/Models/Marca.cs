using System.Collections.Generic;

namespace MSVenta.Venta.Models
{
    public class Marca
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Relación con Productos
        public ICollection<Producto> Productos { get; set; }
    }
}