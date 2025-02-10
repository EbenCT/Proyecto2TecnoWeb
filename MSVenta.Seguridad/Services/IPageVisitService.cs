using MSVenta.Seguridad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSVenta.Seguridad.Services
{
    public interface IPageVisitService
    {
        Task<PageVisit> CreatePageVisit(PageVisit pageVisit);
        Task<PageVisit> GetPageVisitById(int id);
        Task<IEnumerable<PageVisit>> GetAllPageVisits();
    }
}
