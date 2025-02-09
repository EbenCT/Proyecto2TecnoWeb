using System.Text.Json.Serialization;

namespace MSVenta.Venta.Models
{
    public class DetalleAjuste
    {
        public int id { get; set; }
        public int id_ajuste_inventario { get; set; }
        [JsonIgnore]
        public AjusteInventario AjusteInventario { get; set; }
        public int id_producto_almacen { get; set; }
        public ProductoAlmacen ProductoAlmacen { get; set; }
        public int cantidad { get; set; }
    }
}