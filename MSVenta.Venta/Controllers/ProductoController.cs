using Microsoft.AspNetCore.Mvc;
using MSVenta.Venta.Models;
using MSVenta.Venta.Services;
using System.Threading.Tasks;

namespace MSVenta.Venta.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService) => _productoService = productoService;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _productoService.GetAllProductos());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _productoService.GetProducto(id));

        [HttpPost]
        public async Task<IActionResult> Create(Producto producto)
        {
            await _productoService.CreateProducto(producto);
            return CreatedAtAction(nameof(Get), new { id = producto.Id }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Producto producto)
        {
            if (id != producto.Id) return BadRequest();
            await _productoService.UpdateProducto(producto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productoService.DeleteProducto(id);
            return NoContent();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
