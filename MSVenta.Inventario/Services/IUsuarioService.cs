using System.Threading.Tasks;

namespace MSVenta.Venta.Services
{
    public interface IUsuarioService
    {
        Task<bool> ValidateUsuario(int usuario_id);
    }
}
