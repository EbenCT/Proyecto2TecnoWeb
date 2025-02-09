using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class AjusteInventarioController : ControllerBase
{
    private readonly AjusteInventarioService _ajusteInventarioService;

    public AjusteInventarioController(AjusteInventarioService ajusteInventarioService)
    {
        _ajusteInventarioService = ajusteInventarioService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AjusteInventario>>> GetAll()
    {
        var ajustes = await _ajusteInventarioService.GetAllAsync();
        return Ok(ajustes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AjusteInventario>> GetById(int id)
    {
        var ajuste = await _ajusteInventarioService.GetByIdAsync(id);
        if (ajuste == null)
            return NotFound();

        return Ok(ajuste);
    }

    [HttpPost]
    public async Task<ActionResult<AjusteInventario>> Create(AjusteInventarioDTO ajusteDTO)
    {
        try
        {
            var ajuste = new AjusteInventario
            {
                fecha = DateTime.Now,
                descripcion = ajusteDTO.descripcion,
                tipo = ajusteDTO.tipo,
                usuarioId = ajusteDTO.usuarioId
            };

            var resultado = await _ajusteInventarioService.CrearAjusteInventarioAsync(ajuste, ajusteDTO.Detalles);
            return CreatedAtAction(nameof(GetById), new { id = resultado.id }, resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AjusteInventario>> Update(int id, AjusteInventarioDTO ajusteDTO)
    {
        try
        {
            var ajuste = new AjusteInventario
            {
                descripcion = ajusteDTO.descripcion,
                tipo = ajusteDTO.tipo,
                usuarioId = ajusteDTO.usuarioId
            };

            var resultado = await _ajusteInventarioService.UpdateAjusteInventarioAsync(id, ajuste, ajusteDTO.Detalles);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _ajusteInventarioService.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}