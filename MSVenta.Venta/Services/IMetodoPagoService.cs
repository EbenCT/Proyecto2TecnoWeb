using System.Collections.Generic;
using System.Threading.Tasks;
using MSVenta.Venta.Models;

namespace MSVenta.Venta.Services
{
    public interface IMetodoPagoService
    {
        Task<IEnumerable<MetodoPago>> GetAllMetodosPago();
        Task<MetodoPago> GetMetodoPago(int id);
        Task<MetodoPago> CreateMetodoPago(MetodoPago metodoPago);
        Task<bool> UpdateMetodoPago(int id, MetodoPago metodoPago);
        Task<bool> DeleteMetodoPago(int id);
    }
}
