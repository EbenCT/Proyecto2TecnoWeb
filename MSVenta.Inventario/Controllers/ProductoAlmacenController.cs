using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Venta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoAlmacenController : Controller
    {
        private readonly ProductoAlmacenService _productoAlmacenService;
        private readonly IProductoAlmacenHttpClient _productoAlmacenHttpClient;
        private readonly ILogger<ProductoAlmacenController> _logger;

        public ProductoAlmacenController(
            ProductoAlmacenService productoAlmacenService,
            IProductoAlmacenHttpClient productoAlmacenHttpClient,
            ILogger<ProductoAlmacenController> logger)
        {
            _productoAlmacenService = productoAlmacenService;
            _productoAlmacenHttpClient = productoAlmacenHttpClient;
            _logger = logger;
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
        [HttpPost]
        public async Task<IActionResult> Create(ProductoAlmacen productoAlmacen)
        {
            try
            {
                // Crear en Inventario
                await _productoAlmacenService.AddAsync(productoAlmacen);

                // Sincronizar con Ventas
                var (syncSuccess, errorMessage) = await _productoAlmacenHttpClient.SyncProductoAlmacenAsync(productoAlmacen);
                if (!syncSuccess)
                {
                    // Rollback de la creación en Inventario
                    await _productoAlmacenService.DeleteAsync(productoAlmacen.id);

                    return StatusCode(500, new
                    {
                        Message = "Error al sincronizar la relación ProductoAlmacen con el servicio de Ventas",
                        Details = errorMessage,
                        Id= productoAlmacen.id,
                        ProductoId = productoAlmacen.ProductoId,
                        AlmacenId = productoAlmacen.AlmacenId
                    });
                }

                return CreatedAtAction(nameof(GetById), new { id = productoAlmacen.id }, productoAlmacen);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear ProductoAlmacen: {ex.Message}");
                return StatusCode(500, new
                {
                    Message = "Error al crear la relación ProductoAlmacen",
                    Details = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductoAlmacen productoAlmacen)
        {
            if (id != productoAlmacen.id)
                return BadRequest("ID mismatch");

            // Actualizar en Inventario
            var updated = await _productoAlmacenService.UpdateAsync(productoAlmacen);
            if (!updated)
                return NotFound();

            // Sincronizar con Ventas
            var syncSuccess = await _productoAlmacenHttpClient.UpdateProductoAlmacenAsync(
                id,
                productoAlmacen
            );

            if (!syncSuccess)
            {
                return StatusCode(500, new
                {
                    Message = "ProductoAlmacen se actualizó en Inventario pero no se pudo sincronizar con Ventas"
                });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productoAlmacenService.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            // Sincronizar con Ventas
            var syncSuccess = await _productoAlmacenHttpClient.DeleteProductoAlmacenAsync(
                id
            );

            if (!syncSuccess)
            {
                return StatusCode(500, new
                {
                    Message = "ProductoAlmacen se elimino en Inventario pero no se pudo sincronizar con Ventas"
                });
            }
            return NoContent();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
