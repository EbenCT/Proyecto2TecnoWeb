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
        private readonly ICategoriaHttpClient _categoriaHttpClient;

        public CategoriaController(
            ICategoriaService categoriaService,
            ICategoriaHttpClient categoriaHttpClient)
        {
            _categoriaService = categoriaService;
            _categoriaHttpClient = categoriaHttpClient;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _categoriaService.GetAllCategorias());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _categoriaService.GetCategoria(id));

        [HttpPost]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            // Crear en Inventario
            await _categoriaService.CreateCategoria(categoria);

            // Sincronizar con Ventas
            var syncSuccess = await _categoriaHttpClient.SyncCategoriaAsync(categoria);

            if (!syncSuccess)
            {
                return StatusCode(500, new { Message = "La categoría se creó en Inventario pero no se pudo sincronizar con Ventas" });
            }

            return CreatedAtAction(nameof(Get), new { id = categoria.id }, categoria);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Categoria categoria)
        {
            if (id != categoria.id) return BadRequest();

            // Actualizar en Inventario
            await _categoriaService.UpdateCategoria(categoria);

            // Sincronizar con Ventas
            var syncSuccess = await _categoriaHttpClient.UpdateCategoriaAsync(id, categoria);

            if (!syncSuccess)
            {
                return StatusCode(500, new { Message = "La categoría se actualizó en Inventario pero no se pudo sincronizar con Ventas" });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Eliminar en Inventario
            await _categoriaService.DeleteCategoria(id);

            // Sincronizar con Ventas
            var syncSuccess = await _categoriaHttpClient.DeleteCategoriaAsync(id);

            if (!syncSuccess)
            {
                return StatusCode(500, new { Message = "La categoría se eliminó en Inventario pero no se pudo sincronizar con Ventas" });
            }

            return NoContent();
        }
    }
}
