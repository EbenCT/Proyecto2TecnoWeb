using Microsoft.AspNetCore.Mvc;
using MSVenta.Seguridad.Models;
using MSVenta.Seguridad.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Seguridad.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PageVisitController : ControllerBase
    {
        private readonly IPageVisitService _service;

        public PageVisitController(IPageVisitService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PageVisit>>> GetAll()
        {
            var visits = await _service.GetAllPageVisits();
            return Ok(visits);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PageVisit>> GetById(int id)
        {
            var visit = await _service.GetPageVisitById(id);
            return visit == null ? NotFound() : Ok(visit);
        }

        [HttpPost]
        public async Task<ActionResult<PageVisit>> Create(PageVisit pageVisit)
        {
            var createdVisit = await _service.CreatePageVisit(pageVisit);
            return CreatedAtAction(nameof(GetById), new { id = createdVisit.Id }, createdVisit);
        }
    }
}
