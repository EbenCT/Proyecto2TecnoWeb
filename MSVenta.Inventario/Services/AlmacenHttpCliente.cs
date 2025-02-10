using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MSVenta.Venta.Models;

namespace MSVenta.Venta.Services
{
    public interface IAlmacenHttpClient
    {
        Task<(bool success, string errorMessage)> SyncAlmacenAsync(Almacen almacen);
        Task<bool> UpdateAlmacenAsync(int id, Almacen almacen);
        Task<bool> DeleteAlmacenAsync(int id);
    }

    public class AlmacenHttpClient : IAlmacenHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AlmacenHttpClient> _logger;

        public AlmacenHttpClient(HttpClient httpClient, ILogger<AlmacenHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(bool success, string errorMessage)> SyncAlmacenAsync(Almacen almacen)
        {
            try
            {
                var json = JsonSerializer.Serialize(almacen, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                _logger.LogInformation($"Intentando sincronizar almacén. URL: {_httpClient.BaseAddress}api/almacen");
                _logger.LogInformation($"Datos enviados: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/almacen", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var statusCode = (int)response.StatusCode;
                    var errorMessage = $"Error HTTP {statusCode}. URL: {_httpClient.BaseAddress}api/almacen. Respuesta: {errorContent}";
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

        public async Task<bool> UpdateAlmacenAsync(int id, Almacen almacen)
        {
            try
            {
                var json = JsonSerializer.Serialize(almacen, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/almacen/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error al actualizar almacén {id}: {response.StatusCode}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar almacén {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAlmacenAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/almacen/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error al eliminar almacén {id}: {response.StatusCode}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar almacén {id}: {ex.Message}");
                return false;
            }
        }
    }
}