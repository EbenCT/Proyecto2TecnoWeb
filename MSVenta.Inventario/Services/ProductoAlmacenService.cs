using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Services
{
    public class ProductoAlmacenService
    {
        private readonly ContextDatabase _context;

        public ProductoAlmacenService(ContextDatabase context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductoAlmacen>> GetAllAsync()
        {
            return await _context.ProductosAlmacenes
                .Include(pa => pa.Producto)
                .Include(pa => pa.Almacen)
                .ToListAsync();
        }

        public async Task<ProductoAlmacen> GetByIdAsync(int id)
        {
            return await _context.ProductosAlmacenes
                .Include(pa => pa.Producto)
                .Include(pa => pa.Almacen)
                .FirstOrDefaultAsync(pa => pa.id == id);
        }

        public async Task<ProductoAlmacen> AddAsync(ProductoAlmacen productoAlmacen)
        {
            _context.ProductosAlmacenes.Add(productoAlmacen);
            await _context.SaveChangesAsync();
            return productoAlmacen;
        }

        public async Task<bool> UpdateAsync(ProductoAlmacen productoAlmacen)
        {
            var existingProductoAlmacen = await _context.ProductosAlmacenes.FindAsync(productoAlmacen.id);
            if (existingProductoAlmacen == null) return false;

            existingProductoAlmacen.id_producto = productoAlmacen.id_producto;
            existingProductoAlmacen.id_almacen = productoAlmacen.id_almacen;
            existingProductoAlmacen.stock = productoAlmacen.stock;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var productoAlmacen = await _context.ProductosAlmacenes.FindAsync(id);
            if (productoAlmacen == null) return false;

            _context.ProductosAlmacenes.Remove(productoAlmacen);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
