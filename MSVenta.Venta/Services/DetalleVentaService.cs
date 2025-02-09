using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MSVenta.Venta.Repositories;
using System.Linq;

namespace MSVenta.Venta.Services
{
    public class DetalleVentaService : IDetalleVentaService
    {
        private readonly ContextDatabase _context;

        public DetalleVentaService(ContextDatabase context) => _context = context;

        public async Task<IEnumerable<DetalleVenta>> GetAllDetalles()
            => await _context.DetallesVenta
                .Include(dv => dv.Venta)
                .Include(dv => dv.ProductoAlmacen)
                .ToListAsync();

        public async Task<DetalleVenta> GetDetalle(int id)
        {
            return await _context.DetallesVenta
                  .Include(dv => dv.Venta)
                    .ThenInclude(c => c.Cliente)
                  .Include(dv => dv.ProductoAlmacen)
                    .ThenInclude(p => p.Producto)
                        .ThenInclude(c => c.Categoria)
                  .Include(dv => dv.ProductoAlmacen)
                    .ThenInclude(a => a.Almacen)
                  .FirstOrDefaultAsync(dv => dv.Id == id);
        }

        public async Task<List<DetalleVenta>> GetDetallesPorVenta(int ventaId)
        {
            return await _context.DetallesVenta
                  .Where(dv => dv.VentaId == ventaId)
                  .Include(dv => dv.Venta)
                    .ThenInclude(v => v.Cliente)
                  .Include(dv => dv.ProductoAlmacen)
                    .ThenInclude(pa => pa.Producto)
                        .ThenInclude(p => p.Categoria)
                  .Include(dv => dv.ProductoAlmacen)
                    .ThenInclude(pa => pa.Almacen)
                  .ToListAsync();
        }


        public async Task CreateDetalle(DetalleVenta detalle)
        {
            // Validar existencia de Venta
            var venta = await _context.Ventas.FindAsync(detalle.VentaId);
            if (venta == null)
                throw new Exception("Venta no existe");

            // Validar existencia del Producto en Almacén
            var productoAlmacen = await _context.ProductosAlmacenes.FindAsync(detalle.ProductoAlmacenId);
            if (productoAlmacen == null)
                throw new Exception("Producto en almacén no existe");

            // Verificar si hay stock suficiente
            if (productoAlmacen.Stock < detalle.Cantidad)
                throw new Exception("Stock insuficiente para el producto en almacén");

            // Descontar el stock
            productoAlmacen.Stock -= detalle.Cantidad;

            // Agregar el detalle de venta
            await _context.DetallesVenta.AddAsync(detalle);
            await _context.SaveChangesAsync();

            // Actualizar el monto_total sumando todos los montos de los detalles de la venta
            venta.MontoTotal = await _context.DetallesVenta
                .Where(dv => dv.VentaId == detalle.VentaId)
                .SumAsync(dv => dv.Monto);

            _context.Ventas.Update(venta);
            _context.ProductosAlmacenes.Update(productoAlmacen);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateDetalle(DetalleVenta detalle)
        {
            _context.Entry(detalle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDetalle(int id)
        {
            var detalle = await _context.DetallesVenta.FindAsync(id);
            _context.DetallesVenta.Remove(detalle);
            await _context.SaveChangesAsync();
        }
    }
}
