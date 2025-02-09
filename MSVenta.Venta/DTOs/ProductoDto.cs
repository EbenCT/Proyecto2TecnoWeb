namespace MSVenta.Venta.DTOs
{
    public class ProductoDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public string Categoria { get; set; }
        public string Marca { get; set; }
        public int Stock { get; set; }
    }
}
