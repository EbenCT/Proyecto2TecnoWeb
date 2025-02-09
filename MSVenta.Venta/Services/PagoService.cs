using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSVenta.Venta.Models;
using MSVenta.Venta.Repositories;

namespace MSVenta.Venta.Services
{
        // Implementation for Pago Service
    public class PagoService : IPagoService
    {
        private readonly ContextDatabase _context;

        public PagoService(ContextDatabase context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Pago>> GetAllPagos()
        {
            return await _context.Pagos
                .Include(p => p.MetodoPago)
                .Include(p => p.Venta)
                .ToListAsync();
        }

        public async Task<Pago> GetPagoById(int pagoId)
        {
            var pago = await _context.Pagos
                .Include(p => p.MetodoPago)
                .Include(p => p.Venta)
                .FirstOrDefaultAsync(p => p.Id == pagoId);

            if (pago == null)
                throw new Exception("Pago no encontrado");

            return pago;
        }

        public async Task<IEnumerable<Pago>> GetPagosByVenta(int ventaId)
        {
            return await _context.Pagos
                .Include(p => p.MetodoPago)
                .Where(p => p.VentaId == ventaId)
                .ToListAsync();
        }

        public async Task<Pago> CrearPagoPendiente(int ventaId, decimal monto)
        {
            var pago = new Pago
            {
                VentaId = ventaId,
                Monto = monto,
                Estado = false,
                FechaPago = null,
                MetodoPagoId = null
            };

            await _context.Pagos.AddAsync(pago);
            await _context.SaveChangesAsync();

            return pago;
        }

        public async Task<Pago> PagarPendiente(int pagoId, decimal montoRecibido, int metodoPagoId)
        {
            var pago = await _context.Pagos.FindAsync(pagoId);

            if (!pago.Estado)
                throw new Exception("Pago no encontrado");

            if (pago.Estado)
                throw new Exception("El pago ya está pagado");

            // Calcular saldo
            decimal saldoPendiente = pago.Monto;
            decimal saldoRestante = saldoPendiente - montoRecibido;

            // Actualizar pago actual
            pago.Estado = saldoRestante <= 0 ? true : false;
            pago.FechaPago = DateTime.Now;
            pago.MetodoPagoId = metodoPagoId;

            // Si hay saldo restante, crear nuevo pago pendiente
            if (saldoRestante > 0)
            {
                var nuevoPago = new Pago
                {
                    VentaId = pago.VentaId,
                    Monto = saldoRestante,
                    Estado = false,
                    FechaPago = null,
                    MetodoPagoId = null
                };
                await _context.Pagos.AddAsync(nuevoPago);
            }

            await _context.SaveChangesAsync();
            return pago;
        }

        public async Task<bool> TieneVentaSaldoPendiente(int ventaId)
        {
            return await _context.Pagos
                .AnyAsync(p => p.VentaId == ventaId && p.Estado == false);
        }
    }
}