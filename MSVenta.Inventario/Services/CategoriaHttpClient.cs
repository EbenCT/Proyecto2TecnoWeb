using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MSVenta.Venta.Models;

namespace MSVenta.Venta.Services
{
    public interface ICategoriaHttpClient
    {
        Task<bool> SyncCategoriaAsync(Categoria categoria);
        Task<bool> UpdateCategoriaAsync(int id, Categoria categoria);
        Task<bool> DeleteCategoriaAsync(int id);
    }

    public class CategoriaHttpClient : ICategoriaHttpClient
    {
        private readonly HttpClient _httpClient;

        public CategoriaHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SyncCategoriaAsync(Categoria categoria)
        {
            var json = JsonSerializer.Serialize(categoria);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/categoria", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategoriaAsync(int id, Categoria categoria)
        {
            var json = JsonSerializer.Serialize(categoria);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/categoria/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoriaAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/categoria/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}