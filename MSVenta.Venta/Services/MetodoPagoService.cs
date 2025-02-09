using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;

namespace MSVenta.Venta.Services
{
    // Implementation for MetodoPago Service
    public class MetodoPagoService : IMetodoPagoService
    {
        private readonly ContextDatabase _context;

        public MetodoPagoService(ContextDatabase context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MetodoPago>> GetAllMetodosPago()
        {
            return await _context.MetodosPago.ToListAsync();
        }

        public async Task<MetodoPago> GetMetodoPago(int id)
        {
            return await _context.MetodosPago.FindAsync(id);
        }
    }

}