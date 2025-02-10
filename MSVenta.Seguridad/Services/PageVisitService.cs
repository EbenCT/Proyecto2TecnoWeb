using Microsoft.EntityFrameworkCore;
using MSVenta.Seguridad.Models;
using MSVenta.Seguridad.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Seguridad.Services
{
    public class PageVisitService : IPageVisitService
    {
        private readonly ContextDatabase _context;

        public PageVisitService(ContextDatabase context)
        {
            _context = context;
        }

        public async Task<PageVisit> CreatePageVisit(PageVisit pageVisit)
        {
            // Buscar si ya existe un registro con el mismo nombre de página
            var existingPageVisit = await _context.PageVisits
                                                  .FirstOrDefaultAsync(pv => pv.PageName == pageVisit.PageName);

            if (existingPageVisit != null)
            {
                // Si ya existe, incrementar el contador y actualizar la BD
                existingPageVisit.VisitCount += 1;
                _context.PageVisits.Update(existingPageVisit);
                await _context.SaveChangesAsync();
                return existingPageVisit;
            }
            else
            {
                // Si no existe, crear un nuevo registro con VisitCount = 1
                pageVisit.VisitCount = 1;
                _context.PageVisits.Add(pageVisit);
                await _context.SaveChangesAsync();
                return pageVisit;
            }
        }

        public async Task<PageVisit> GetPageVisitById(int id)
        {
            return await _context.PageVisits.FindAsync(id);
        }

        public async Task<IEnumerable<PageVisit>> GetAllPageVisits()
        {
            return await _context.PageVisits.ToListAsync();
        }
    }
}
