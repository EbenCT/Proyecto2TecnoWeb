using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Services
{
    public class ProductoService : IProductoService
    {
        private readonly ContextDatabase _context;

        public ProductoService(ContextDatabase context) => _context = context;

        public async Task<IEnumerable<Producto>> GetAllProductos()
            => await _context.Productos.ToListAsync();

        public async Task<Producto> GetProducto(int id)
            => await _context.Productos.FirstOrDefaultAsync(p => p.id == id);

        public async Task CreateProducto(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProducto(Producto producto)
        {
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }
}
