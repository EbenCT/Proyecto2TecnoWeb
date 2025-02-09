using MSVenta.Venta.Models;
using System.Collections.Generic;

public class AjusteInventarioDTO
{
    public string descripcion { get; set; }
    public int tipo { get; set; } //  (0 = Egreso, 1 = Ingreso)
    public int usuarioId { get; set; }
    public List<DetalleAjusteDTO> Detalles { get; set; }
}

public class DetalleAjusteDTO
{
    public int id_producto_almacen { get; set; }
    public int cantidad { get; set; }
}