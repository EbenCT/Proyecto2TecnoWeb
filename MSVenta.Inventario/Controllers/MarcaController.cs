using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;
using System;
using System.Threading.Tasks;

namespace MSVenta.Venta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarcaController : ControllerBase
    {
        private readonly IMarcaService _marcaService;
        private readonly IMarcaHttpClient _marcaHttpClient;
        public MarcaController(
            IMarcaService marcaService,
            IMarcaHttpClient marcaHttpClient)
        {
            _marcaService = marcaService;
            _marcaHttpClient = marcaHttpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _marcaService.GetAllMarcas());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _marcaService.GetMarca(id));

        [HttpPost]
        public async Task<IActionResult> Create(Marca marca)
        {
            // Primero creamos la marca en el microservicio de Inventario
            await _marcaService.CreateMarca(marca);

            // Luego sincronizamos con el microservicio de Ventas
            var syncSuccess = await _marcaHttpClient.SyncMarcaAsync(marca);

            if (!syncSuccess)
            {
                // Aquí podrías implementar un mecanismo de compensación si la sincronización falla
                // Por ejemplo, registrar el error, intentar nuevamente, o revertir la operación
                return StatusCode(500, new { Message = "La marca se creó en Inventario pero no se pudo sincronizar con Ventas" });
            }

            return CreatedAtAction(nameof(Get), new { id = marca.id }, marca);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Marca marca)
        {
            try
            {
                if (id != marca.id) return BadRequest();

                // Actualizar en Inventario
                await _marcaService.UpdateMarca(marca);

                // Sincronizar con Ventas
                var syncSuccess = await _marcaHttpClient.UpdateMarcaAsync(id, marca);

                if (!syncSuccess)
                {
                    return StatusCode(500, new { Message = "La marca se actualizó en Inventario pero no se pudo sincronizar con Ventas" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error interno al actualizar la marca" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Eliminar en Inventario
                await _marcaService.DeleteMarca(id);

                // Sincronizar con Ventas
                var syncSuccess = await _marcaHttpClient.DeleteMarcaAsync(id);

                if (!syncSuccess)
                {
                    return StatusCode(500, new { Message = "La marca se eliminó en Inventario pero no se pudo sincronizar con Ventas" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error interno al eliminar la marca" });
            }
        }
    }
}