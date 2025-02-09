using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ContextDatabase _context;

        public CategoriaService(ContextDatabase context) => _context = context;

        public async Task<IEnumerable<Categoria>> GetAllCategorias() =>
            await _context.Categorias
                .Include(c => c.Productos)  // Añade esta línea para incluir los productos
                .ToListAsync();

        public async Task<Categoria> GetCategoria(int id) =>
            await _context.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.id == id);

        public async Task CreateCategoria(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoria(Categoria categoria)
        {
            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }
}
