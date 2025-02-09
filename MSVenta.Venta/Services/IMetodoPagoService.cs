
using System.Collections.Generic;

using System.Threading.Tasks;

using MSVenta.Venta.Models;


namespace MSVenta.Venta.Services
{
    // Interfaces
    public interface IMetodoPagoService
    {
        Task<IEnumerable<MetodoPago>> GetAllMetodosPago();
        Task<MetodoPago> GetMetodoPago(int id);
    }

}