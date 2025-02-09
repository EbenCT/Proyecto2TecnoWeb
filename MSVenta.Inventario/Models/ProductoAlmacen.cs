namespace MSVenta.Venta.Models
{
    public class ProductoAlmacen
    {
        public int id { get; set; }
        public int id_producto { get; set; }
        public Producto Producto { get; set; }
        public int id_almacen { get; set; }
        public Almacen Almacen { get; set; }
        public int stock { get; set; }
    }
}
