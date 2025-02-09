using MSVenta.Venta.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Services
{
    public interface IMarcaService
    {
        Task<IEnumerable<Marca>> GetAllMarcas();
        Task<Marca> GetMarca(int id);
        Task CreateMarca(Marca marca);
        Task UpdateMarca(Marca marca);
        Task DeleteMarca(int id);
    }
}