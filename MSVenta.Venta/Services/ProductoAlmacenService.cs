using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.DTOs;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;
using System.Collections.Generic;
using System.Linq;
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
                    .ThenInclude(c => c.Categoria)
                .Include(pa => pa.Almacen)
                .ToListAsync();

        }

        public async Task<ProductoAlmacen> GetByIdAsync(int id)
        {
            return await _context.ProductosAlmacenes
                .Include(pa => pa.Producto) // Incluye el producto
                    .ThenInclude(p => p.Categoria) // Luego, incluye la categoría del producto
                .Include(pa => pa.Almacen) // Incluye el almacén
                .FirstOrDefaultAsync(pa => pa.Id == id);



        }


        public async Task<ProductoAlmacen> AddAsync(ProductoAlmacen productoAlmacen)
        {
            _context.ProductosAlmacenes.Add(productoAlmacen);
            await _context.SaveChangesAsync();
            return productoAlmacen;
        }

        public async Task<bool> UpdateAsync(ProductoAlmacen productoAlmacen)
        {
            var existingProductoAlmacen = await _context.ProductosAlmacenes.FindAsync(productoAlmacen.Id);
            if (existingProductoAlmacen == null) return false;

            existingProductoAlmacen.ProductoId = productoAlmacen.ProductoId;
            existingProductoAlmacen.AlmacenId = productoAlmacen.AlmacenId;
            existingProductoAlmacen.Stock = productoAlmacen.Stock;

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

        public async Task<AlmacenConProductosDto> GetAlmacenConProductosAsync(int almacenId)
        {
            // Obtener todos los productos relacionados con el almacén filtrado por AlmacenId
            var productosAlmacen = await _context.ProductosAlmacenes
                .Where(pa => pa.AlmacenId == almacenId)  // Filtrar por AlmacenId
                .Include(pa => pa.Producto)  // Incluir la entidad Producto asociada
                    .ThenInclude(p => p.Categoria)  // Incluir la entidad Categoria asociada al Producto
                .Include(pa => pa.Almacen)  // Incluir la entidad Almacen
                .ToListAsync();  // Devolver todos los registros encontrados

            // Verificar si no se encuentran registros
            if (productosAlmacen == null || !productosAlmacen.Any())
            {
                return null; // Si no se encuentra el almacén o productos, devuelve null
            }

            // Obtener la información del Almacén (usamos el primer elemento, ya que todos los productos tienen el mismo AlmacenId)
            var almacen = productosAlmacen.FirstOrDefault()?.Almacen;

            // Verificar si el almacén existe
            if (almacen == null)
            {
                return null; // Si no se encuentra el almacén, devuelve null
            }

            // Mapeamos los datos a un DTO (Data Transfer Object) que podemos devolver
            var almacenConProductosDto = new AlmacenConProductosDto
            {
                AlmacenId = almacen.Id,
                AlmacenNombre = almacen.Nombre,
                Productos = productosAlmacen.Select(pa => new ProductoDto
                {
                    ProductoId = pa.ProductoId,
                    Nombre = pa.Producto.Nombre,
                    Precio = (decimal)pa.Producto.Precio,
                    Categoria = pa.Producto.Categoria.Nombre,
                    Stock = pa.Stock
                }).ToList()
            };

            return almacenConProductosDto;
        }
    }
    }
