using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Services;

namespace MSVenta.Venta.Controllers
{
    // Método de Pago Controller
    [ApiController]
    [Route("api/[controller]")]
    public class MetodoPagoController : ControllerBase
    {
        private readonly IMetodoPagoService _metodoPagoService;

        public MetodoPagoController(IMetodoPagoService metodoPagoService)
        {
            _metodoPagoService = metodoPagoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var metodosPago = await _metodoPagoService.GetAllMetodosPago();
            return Ok(metodosPago);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var metodoPago = await _metodoPagoService.GetMetodoPago(id);

            if (metodoPago == null)
                return NotFound();

            return Ok(metodoPago);
        }
    }
}