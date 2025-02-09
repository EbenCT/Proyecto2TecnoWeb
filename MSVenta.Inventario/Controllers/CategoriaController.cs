using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;
using System.Threading.Tasks;

namespace MSVenta.Venta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService) => _categoriaService = categoriaService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _categoriaService.GetAllCategorias());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _categoriaService.GetCategoria(id));

        [HttpPost]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            await _categoriaService.CreateCategoria(categoria);
            return CreatedAtAction(nameof(Get), new { id = categoria.id }, categoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Categoria categoria)
        {
            if (id != categoria.id) return BadRequest();
            await _categoriaService.UpdateCategoria(categoria);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoriaService.DeleteCategoria(id);
            return NoContent();
        }
    }
}
