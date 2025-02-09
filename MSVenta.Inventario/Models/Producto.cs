namespace MSVenta.Venta.Models
{
    public class Producto
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public double precio { get; set; }

        // Relación con Categoría
        public int categoriaId { get; set; }
        // Relación con Marca
        public int marcaId { get; set; }
    }
}
