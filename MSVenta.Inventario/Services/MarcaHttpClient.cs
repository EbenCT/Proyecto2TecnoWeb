using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MSVenta.Venta.Models;

namespace MSVenta.Venta.Services
{
    public interface IMarcaHttpClient
    {
        Task<bool> SyncMarcaAsync(Marca marca);
        Task<bool> UpdateMarcaAsync(int id, Marca marca);
        Task<bool> DeleteMarcaAsync(int id);
    }

    public class MarcaHttpClient : IMarcaHttpClient
    {
        private readonly HttpClient _httpClient;

        public MarcaHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> SyncMarcaAsync(Marca marca)
        {
            var json = JsonSerializer.Serialize(marca);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Llamada al endpoint de creación de marca en el microservicio de Ventas
            var response = await _httpClient.PostAsync("api/marca", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> UpdateMarcaAsync(int id, Marca marca)
        {
            try
            {
                var json = JsonSerializer.Serialize(marca);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/marca/{id}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteMarcaAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/marca/{id}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}