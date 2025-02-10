using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MSVenta.Venta.Models;

namespace MSVenta.Venta.Services
{
    public interface IProductoAlmacenHttpClient
    {
        Task<(bool success, string errorMessage)> SyncProductoAlmacenAsync(ProductoAlmacen productoAlmacen);
        Task<bool> UpdateProductoAlmacenAsync(int id, ProductoAlmacen productoAlmacen);
        Task<bool> DeleteProductoAlmacenAsync(int id);
    }

    public class ProductoAlmacenHttpClient : IProductoAlmacenHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductoAlmacenHttpClient> _logger;

        public ProductoAlmacenHttpClient(HttpClient httpClient, ILogger<ProductoAlmacenHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(bool success, string errorMessage)> SyncProductoAlmacenAsync(ProductoAlmacen productoAlmacen)
        {
            try
            {
                var json = JsonSerializer.Serialize(productoAlmacen, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                _logger.LogInformation($"Intentando sincronizar ProductoAlmacen. URL: {_httpClient.BaseAddress}api/productoalmacen");
                _logger.LogInformation($"Datos enviados: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/productoalmacen", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var statusCode = (int)response.StatusCode;
                    var errorMessage = $"Error HTTP {statusCode}. URL: {_httpClient.BaseAddress}api/productoalmacen. Respuesta: {errorContent}";
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

        public async Task<bool> UpdateProductoAlmacenAsync(int id, ProductoAlmacen productoAlmacen)
        {
            try
            {
                var json = JsonSerializer.Serialize(productoAlmacen, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/productoalmacen/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error al actualizar ProductoAlmacen (Producto: {id}): {response.StatusCode}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar ProductoAlmacen (Producto: {id}): {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProductoAlmacenAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/productoalmacen/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error al eliminar ProductoAlmacen (Producto: {id}): {response.StatusCode}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar ProductoAlmacen (Producto: {id}): {ex.Message}");
                return false;
            }
        }
    }
}