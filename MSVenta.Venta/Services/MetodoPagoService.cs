using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;

namespace MSVenta.Venta.Services
{
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

        public async Task<MetodoPago> CreateMetodoPago(MetodoPago metodoPago)
        {
            _context.MetodosPago.Add(metodoPago);
            await _context.SaveChangesAsync();
            return metodoPago;
        }

        public async Task<bool> UpdateMetodoPago(int id, MetodoPago metodoPago)
        {
            var existingMetodoPago = await _context.MetodosPago.FindAsync(id);
            if (existingMetodoPago == null)
                return false;

            existingMetodoPago.Nombre = metodoPago.Nombre;
            //existingMetodoPago.Descripcion = metodoPago.Descripcion;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMetodoPago(int id)
        {
            var metodoPago = await _context.MetodosPago.FindAsync(id);
            if (metodoPago == null)
                return false;

            _context.MetodosPago.Remove(metodoPago);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
