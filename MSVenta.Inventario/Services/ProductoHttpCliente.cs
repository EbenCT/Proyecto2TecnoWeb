using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MSVenta.Venta.Models;

namespace MSVenta.Venta.Services
{
    public interface IProductoHttpClient
    {
        Task<(bool success, string errorMessage)> SyncProductoAsync(Producto producto);
        Task<bool> UpdateProductoAsync(int id, Producto producto);
        Task<bool> DeleteProductoAsync(int id);
    }

    public class ProductoHttpClient : IProductoHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductoHttpClient> _logger;

        public ProductoHttpClient(HttpClient httpClient, ILogger<ProductoHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(bool success, string errorMessage)> SyncProductoAsync(Producto producto)
        {
            try
            {
                var json = JsonSerializer.Serialize(producto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                _logger.LogInformation($"Intentando sincronizar producto. URL: {_httpClient.BaseAddress}api/producto");
                _logger.LogInformation($"Datos enviados: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/producto", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var statusCode = (int)response.StatusCode;
                    var errorMessage = $"Error HTTP {statusCode}. URL: {_httpClient.BaseAddress}api/producto. Respuesta: {errorContent}";
                    _logger.LogError(errorMessage);
                    return (false, errorMessage);
                }

                return (true, null);
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = $"Error de conexión: {ex.Message}. ¿El servicio de Ventas está en ejecución?";
                _logger.LogError(errorMessage);
                return (false, errorMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error inesperado: {ex.Message}";
                _logger.LogError(errorMessage);
                return (false, errorMessage);
            }
        }

        public async Task<bool> UpdateProductoAsync(int id, Producto producto)
        {
            var json = JsonSerializer.Serialize(producto, new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/producto/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/producto/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}