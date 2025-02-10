using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;

namespace MSVenta.Venta.Controllers
{
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MetodoPago metodoPago)
        {
            if (metodoPago == null)
                return BadRequest();

            var createdMetodoPago = await _metodoPagoService.CreateMetodoPago(metodoPago);
            return CreatedAtAction(nameof(Get), new { id = createdMetodoPago.Id }, createdMetodoPago);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MetodoPago metodoPago)
        {
            if (metodoPago == null)
                return BadRequest();

            var updated = await _metodoPagoService.UpdateMetodoPago(id, metodoPago);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _metodoPagoService.DeleteMetodoPago(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
