using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;
using System;
using System.Threading.Tasks;

namespace MSVenta.Venta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlmacenController : Controller
    {
        private readonly IAlmecenService _almacenService;
        private readonly IAlmacenHttpClient _almacenHttpClient;
        private readonly ILogger<AlmacenController> _logger;

        public AlmacenController(
            IAlmecenService almacenService,
            IAlmacenHttpClient almacenHttpClient,
            ILogger<AlmacenController> logger)
        {
            _almacenService = almacenService;
            _almacenHttpClient = almacenHttpClient;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _almacenService.GetAllAlamcenes());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _almacenService.GetAlmacen(id));

        [HttpPost]
        public async Task<IActionResult> Create(Almacen almacen)
        {
            try
            {
                // Crear en Inventario
                await _almacenService.CreateAlmacen(almacen);

                // Sincronizar con Ventas
                var (syncSuccess, errorMessage) = await _almacenHttpClient.SyncAlmacenAsync(almacen);
                if (!syncSuccess)
                {
                    // Rollback de la creación en Inventario
                    await _almacenService.DeleteAlmacen(almacen.id);

                    return StatusCode(500, new
                    {
                        Message = "Error al sincronizar el almacén con el servicio de Ventas",
                        Details = errorMessage,
                        AlmacenId = almacen.id
                    });
                }

                return CreatedAtAction(nameof(Get), new { id = almacen.id }, almacen);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear almacén: {ex.Message}");
                return StatusCode(500, new
                {
                    Message = "Error al crear el almacén",
                    Details = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Almacen almacen)
        {
            if (id != almacen.id) return BadRequest();

            try
            {
                await _almacenService.UpdateAlmacen(almacen);
                var syncSuccess = await _almacenHttpClient.UpdateAlmacenAsync(id, almacen);

                if (!syncSuccess)
                {
                    return StatusCode(500, new
                    {
                        Message = "El almacén se actualizó en Inventario pero no se pudo sincronizar con Ventas"
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar almacén {id}: {ex.Message}");
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _almacenService.DeleteAlmacen(id);
                var syncSuccess = await _almacenHttpClient.DeleteAlmacenAsync(id);

                if (!syncSuccess)
                {
                    return StatusCode(500, new
                    {
                        Message = "El almacén se eliminó en Inventario pero no se pudo sincronizar con Ventas"
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar almacén {id}: {ex.Message}");
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
