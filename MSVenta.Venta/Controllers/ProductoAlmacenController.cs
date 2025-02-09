using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoAlmacenController : Controller
    {
        private readonly ProductoAlmacenService _productoAlmacenService;

        public ProductoAlmacenController(ProductoAlmacenService productoAlmacenService)
        {
            _productoAlmacenService = productoAlmacenService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoAlmacen>>> GetAll()
        {
            return Ok(await _productoAlmacenService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoAlmacen>> GetById(int id)
        {
            var productoAlmacen = await _productoAlmacenService.GetByIdAsync(id);
            if (productoAlmacen == null)
                return NotFound();
            return Ok(productoAlmacen);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoAlmacen>> Create(ProductoAlmacen productoAlmacen)
        {
            var createdProductoAlmacen = await _productoAlmacenService.AddAsync(productoAlmacen);
            return CreatedAtAction(nameof(GetById), new { id = createdProductoAlmacen.Id }, createdProductoAlmacen);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductoAlmacen productoAlmacen)
        {
            if (id != productoAlmacen.Id)
                return BadRequest("ID mismatch");

            var updated = await _productoAlmacenService.UpdateAsync(productoAlmacen);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productoAlmacenService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
