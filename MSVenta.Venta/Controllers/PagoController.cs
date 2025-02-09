using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Services;

namespace MSVenta.Venta.Controllers
{
    // Pago Controller
    [ApiController]
    [Route("api/[controller]")]
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;
        private readonly IVentaService _ventaService;

        public PagoController(IPagoService pagoService, IVentaService ventaService)
        {
            _pagoService = pagoService;
            _ventaService = ventaService;
        }

        // DTO para crear un pago
        public class PagoRequestDto
        {
            public int VentaId { get; set; }
            public decimal Monto { get; set; }
        }

        // DTO para pagar un pago pendiente
        public class PagoPendienteDto
        {
            public int PagoId { get; set; }
            public decimal MontoRecibido { get; set; }
            public int MetodoPagoId { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPagos()
        {
            var pagos = await _pagoService.GetAllPagos();
            return Ok(pagos);
        }

        [HttpGet("{pagoId}")]
        public async Task<IActionResult> GetPagoById(int pagoId)
        {
            try
            {
                var pago = await _pagoService.GetPagoById(pagoId);
                return Ok(pago);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet("venta/{ventaId}")]
        public async Task<IActionResult> GetPagosByVenta(int ventaId)
        {
            // Verificar si la venta existe
            var venta = await _ventaService.GetVenta(ventaId);
            if (venta == null)
                return NotFound("Venta no encontrada");

            var pagos = await _pagoService.GetPagosByVenta(ventaId);
            return Ok(pagos);
        }

        [HttpPost("crear-pendiente")]
        public async Task<IActionResult> CrearPagoPendiente([FromBody] PagoRequestDto request)
        {
            // Verificar si la venta existe
            var venta = await _ventaService.GetVenta(request.VentaId);
            if (venta == null)
                return NotFound("Venta no encontrada");

            var pago = await _pagoService.CrearPagoPendiente(request.VentaId, request.Monto);
            return CreatedAtAction(nameof(GetPagosByVenta), new { ventaId = pago.VentaId }, pago);
        }

        [HttpPost("pagar-pendiente")]
        public async Task<IActionResult> PagarPendiente([FromBody] PagoPendienteDto request)
        {
            try
            {
                var pago = await _pagoService.PagarPendiente(request.PagoId, request.MontoRecibido, request.MetodoPagoId);
                return Ok(pago);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("tiene-saldo-pendiente/{ventaId}")]
        public async Task<IActionResult> TieneVentaSaldoPendiente(int ventaId)
        {
            // Verificar si la venta existe
            var venta = await _ventaService.GetVenta(ventaId);
            if (venta == null)
                return NotFound("Venta no encontrada");

            var tieneSaldoPendiente = await _pagoService.TieneVentaSaldoPendiente(ventaId);
            return Ok(new { tieneSaldoPendiente });
        }
    }
}