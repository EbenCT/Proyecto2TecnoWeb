using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;
using System;
using System.Threading.Tasks;

namespace MSVenta.Venta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly IProductoHttpClient _productoHttpClient;

        public ProductoController(
            IProductoService productoService,
            IProductoHttpClient productoHttpClient)
        {
            _productoService = productoService;
            _productoHttpClient = productoHttpClient;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _productoService.GetAllProductos());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _productoService.GetProducto(id));

        [HttpPost]
        public async Task<IActionResult> Create(Producto producto)
        {
            try
            {
                // Crear en Inventario
                await _productoService.CreateProducto(producto);

                // Sincronizar con Ventas
                var (syncSuccess, errorMessage) = await _productoHttpClient.SyncProductoAsync(producto);
                if (!syncSuccess)
                {
                    // Rollback de la creación en Inventario
                    await _productoService.DeleteProducto(producto.id);

                    return StatusCode(500, new
                    {
                        Message = "Error al sincronizar el producto con el servicio de Ventas",
                        Details = errorMessage,
                        ProductoId = producto.id
                    });
                }

                return CreatedAtAction(nameof(Get), new { id = producto.id }, producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error al crear el producto",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Producto producto)
        {
            if (id != producto.id) return BadRequest();

            // Actualizar en Inventario
            await _productoService.UpdateProducto(producto);

            // Sincronizar con Ventas
            var syncSuccess = await _productoHttpClient.UpdateProductoAsync(id, producto);

            if (!syncSuccess)
            {
                return StatusCode(500, new { Message = "El producto se actualizó en Inventario pero no se pudo sincronizar con Ventas" });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Eliminar en Inventario
            await _productoService.DeleteProducto(id);

            // Sincronizar con Ventas
            var syncSuccess = await _productoHttpClient.DeleteProductoAsync(id);

            if (!syncSuccess)
            {
                return StatusCode(500, new { Message = "El producto se eliminó en Inventario pero no se pudo sincronizar con Ventas" });
            }

            return NoContent();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
