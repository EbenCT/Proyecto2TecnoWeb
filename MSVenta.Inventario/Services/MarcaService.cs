using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly ContextDatabase _context;

        public MarcaService(ContextDatabase context) => _context = context;

        public async Task<IEnumerable<Marca>> GetAllMarcas()
            => await _context.Marcas.Include(c => c.Productos).ToListAsync();

        public async Task<Marca> GetMarca(int id)
            => await _context.Marcas.Include(c => c.Productos).FirstOrDefaultAsync(c => c.id == id);

        public async Task CreateMarca(Marca marca)
        {
            await _context.Marcas.AddAsync(marca);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMarca(Marca marca)
        {
            _context.Entry(marca).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();
        }
    }
}
