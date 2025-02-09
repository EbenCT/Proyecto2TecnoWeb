using System.Collections.Generic;
using System.Threading.Tasks;
using MSVenta.Venta.Models;

namespace MSVenta.Venta.Services
{
    
    public interface IPagoService
    {
        Task<IEnumerable<Pago>> GetPagosByVenta(int ventaId);
        Task<IEnumerable<Pago>> GetAllPagos(); // Obtener todos los pagos
        Task<Pago> GetPagoById(int pagoId);
        Task<Pago> CrearPagoPendiente(int ventaId, decimal monto);
        Task<Pago> PagarPendiente(int pagoId, decimal montoRecibido, int metodoPagoId);
        Task<bool> TieneVentaSaldoPendiente(int ventaId);
    }

}