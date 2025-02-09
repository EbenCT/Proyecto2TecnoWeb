namespace MSVenta.Venta.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public int StockMinimo { get; set; }
        // Relación con Categoría
        public int CategoriaId { get; set; }
        // Relación con Marca
        public int MarcaId { get; set; }// Relación con la categoría
        public Categoria Categoria { get; set; }  // Relación con el objeto Categoria
    }
}
