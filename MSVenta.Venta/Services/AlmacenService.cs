using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Services
{
    public class AlmacenService : IAlmecenService
    {
     
        private readonly ContextDatabase _context;

        public AlmacenService(ContextDatabase context) => _context = context;

        public async Task CreateAlmacen(Almacen almacen)
        {
            await _context.Almacenes.AddAsync(almacen);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAlmacen(int id)
        {
            var almacen = await _context.Almacenes.FindAsync(id);
            _context.Almacenes.Remove(almacen);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Almacen>> GetAllAlamcenes()
        {
            return await _context.Almacenes.ToListAsync(); 
        }

        public async Task<Almacen> GetAlmacen(int id)
        {
            return await _context.Almacenes.FindAsync(id);
        }

        public async Task UpdateAlmacen(Almacen almacen)
        {
            _context.Entry(almacen).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
