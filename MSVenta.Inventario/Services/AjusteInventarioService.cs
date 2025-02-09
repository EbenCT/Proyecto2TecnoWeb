using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Services
{
    public class AjusteInventarioService
    {
        private readonly ContextDatabase _context;

        public AjusteInventarioService(ContextDatabase context)
        {
            _context = context;
        }

        public async Task<AjusteInventario> CrearAjusteInventarioAsync(AjusteInventario ajusteInventario, List<DetalleAjusteDTO> detalles)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.AjustesInventario.Add(ajusteInventario);
                await _context.SaveChangesAsync();

                foreach (var detalle in detalles)
                {
                    var productoAlmacen = await _context.ProductosAlmacenes
                        .FindAsync(detalle.id_producto_almacen);

                    if (productoAlmacen == null)
                        throw new Exception($"Producto almacén no encontrado: {detalle.id_producto_almacen}");

                    int cantidadAjuste = ajusteInventario.tipo == 0 // Egreso
                        ? -Math.Abs(detalle.cantidad)
                        : Math.Abs(detalle.cantidad);

                    if (productoAlmacen.stock == null)
                        throw new Exception($"El stock del producto almacén {productoAlmacen.id} es null");

                    if (ajusteInventario.tipo == 0 && productoAlmacen.stock < Math.Abs(cantidadAjuste))
                        throw new Exception($"Stock insuficiente para el producto en almacén");


                    var detalleAjuste = new DetalleAjuste
                    {
                        id_ajuste_inventario = ajusteInventario.id,
                        id_producto_almacen = detalle.id_producto_almacen,
                        cantidad = cantidadAjuste
                    };

                    _context.DetallesAjuste.Add(detalleAjuste);
                    productoAlmacen.stock += cantidadAjuste;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ajusteInventario;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                if (ex.InnerException != null)
                {
                    throw new Exception($"Error al crear ajuste: {ex.Message} - Inner Exception: {ex.InnerException.Message}", ex.InnerException);
                }
                else
                {
                    throw new Exception($"Error al crear ajuste: {ex.Message}", ex);
                }
            }

        }

        public async Task<IEnumerable<AjusteInventario>> GetAllAsync()
        {
            return await _context.AjustesInventario
                .Include(a => a.DetallesAjuste)
                    .ThenInclude(d => d.ProductoAlmacen)
                        .ThenInclude(pa => pa.Producto)
                .ToListAsync();
        }

        public async Task<AjusteInventario> GetByIdAsync(int id)
        {
            return await _context.AjustesInventario
                .Include(a => a.DetallesAjuste)
                    .ThenInclude(d => d.ProductoAlmacen)
                        .ThenInclude(pa => pa.Producto)
                .FirstOrDefaultAsync(a => a.id == id);
        }

        public async Task<AjusteInventario> UpdateAjusteInventarioAsync(int id, AjusteInventario ajusteInventario, List<DetalleAjusteDTO> detalles)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get existing ajuste with its details
                var existingAjuste = await _context.AjustesInventario
                    .Include(a => a.DetallesAjuste)
                    .FirstOrDefaultAsync(a => a.id == id);

                if (existingAjuste == null)
                    throw new Exception($"Ajuste de inventario no encontrado: {id}");

                // First, revert the original stock changes
                foreach (var detalle in existingAjuste.DetallesAjuste)
                {
                    var productoAlmacen = await _context.ProductosAlmacenes
                        .FindAsync(detalle.id_producto_almacen);

                    if (productoAlmacen != null)
                    {
                        // Revert the original adjustment
                        productoAlmacen.stock -= detalle.cantidad;
                    }
                }

                // Update basic information
                existingAjuste.descripcion = ajusteInventario.descripcion;
                existingAjuste.tipo = ajusteInventario.tipo;
                existingAjuste.usuarioId = ajusteInventario.usuarioId;

                // Remove old details
                _context.DetallesAjuste.RemoveRange(existingAjuste.DetallesAjuste);

                // Add new details
                foreach (var detalle in detalles)
                {
                    var productoAlmacen = await _context.ProductosAlmacenes
                        .FindAsync(detalle.id_producto_almacen);

                    if (productoAlmacen == null)
                        throw new Exception($"Producto almacén no encontrado: {detalle.id_producto_almacen}");

                    int cantidadAjuste = existingAjuste.tipo == 0 // Egreso
                        ? -Math.Abs(detalle.cantidad)
                        : Math.Abs(detalle.cantidad);

                    if (productoAlmacen.stock == null)
                        throw new Exception($"El stock del producto almacén {productoAlmacen.id} es null");

                    if (existingAjuste.tipo == 0 && productoAlmacen.stock < Math.Abs(cantidadAjuste))
                        throw new Exception($"Stock insuficiente para el producto en almacén");

                    var detalleAjuste = new DetalleAjuste
                    {
                        id_ajuste_inventario = existingAjuste.id,
                        id_producto_almacen = detalle.id_producto_almacen,
                        cantidad = cantidadAjuste
                    };

                    _context.DetallesAjuste.Add(detalleAjuste);
                    productoAlmacen.stock += cantidadAjuste;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existingAjuste;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                if (ex.InnerException != null)
                {
                    throw new Exception($"Error al actualizar ajuste: {ex.Message} - Inner Exception: {ex.InnerException.Message}", ex.InnerException);
                }
                else
                {
                    throw new Exception($"Error al actualizar ajuste: {ex.Message}", ex);
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var ajuste = await _context.AjustesInventario
                    .Include(a => a.DetallesAjuste)
                    .FirstOrDefaultAsync(a => a.id == id);

                if (ajuste == null)
                    return false;

                // Revertir los cambios en el stock
                foreach (var detalle in ajuste.DetallesAjuste)
                {
                    var productoAlmacen = await _context.ProductosAlmacenes
                        .FindAsync(detalle.id_producto_almacen);

                    if (productoAlmacen != null)
                    {
                        // Revertir el ajuste (si era suma, restamos; si era resta, sumamos)
                        productoAlmacen.stock -= detalle.cantidad;
                    }
                }

                _context.AjustesInventario.Remove(ajuste);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}