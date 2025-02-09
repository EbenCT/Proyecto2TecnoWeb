using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Services
{
    public class VentaService : IVentaService
    {
        private readonly ContextDatabase _context;

        public VentaService(ContextDatabase context) => _context = context;

        public async Task<IEnumerable<Models.Venta>> GetAllVentas()
        {
            return await _context.Ventas
                .Include(v => v.Cliente) 
                .ToListAsync();
        }

        public async Task<Models.Venta> GetVenta(int id)
        {
            return await _context.Ventas
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task CreateVenta(Models.Venta venta)
        {
            // Verificar si el Cliente existe
            var cliente = await _context.Clientes.FindAsync(venta.ClienteId);
            if (cliente == null)
            {
                throw new ArgumentException("Cliente no existe.");
            }

            // Asignar la entidad relacionada
            venta.Cliente = cliente;

            // Agregar la venta a la base de datos
            await _context.Ventas.AddAsync(venta);
            await _context.SaveChangesAsync();

            // Crear un pago asociado con estado en false
            var pago = new Pago
            {
                VentaId = venta.Id,
                Venta = venta,
                Monto = venta.MontoTotal,
                FechaPago = venta.Fecha, // Asegúrate de que `FechaVenta` existe en tu modelo de Venta
                Descripcion = null,
                Estado = false,
                MetodoPagoId = null
            };

            // Agregar el pago a la base de datos
            await _context.Pagos.AddAsync(pago);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateVenta(Models.Venta venta)
        {
            _context.Entry(venta).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVenta(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();
        }
    }
}
