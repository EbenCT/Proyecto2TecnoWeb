using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;
using System.Threading.Tasks;

namespace MSVenta.Venta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarcaController : ControllerBase
    {
        private readonly IMarcaService _marcaService;

        public MarcaController(IMarcaService marcaService) => _marcaService = marcaService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _marcaService.GetAllMarcas());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _marcaService.GetMarca(id));

        [HttpPost]
        public async Task<IActionResult> Create(Marca marca)
        {
            await _marcaService.CreateMarca(marca);
            return CreatedAtAction(nameof(Get), new { id = marca.Id }, marca);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Marca marca)
        {
            if (id != marca.Id) return BadRequest();
            await _marcaService.UpdateMarca(marca);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _marcaService.DeleteMarca(id);
            return NoContent();
        }
    }
}