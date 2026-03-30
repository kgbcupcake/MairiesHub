using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MairiesHub.Http;
using MairiesHub.Models;

namespace MairiesHub.Services;

public class OllamaService : IOllamaService
{
    public async Task<OllamaResponse?> GetOllamaAsync()
    {
        try
        {
            var response = await SharedHttpClient.Instance.Client.GetAsync("/api/ollama");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<OllamaResponse>(json);
            }
            catch (JsonException)
            {
                return null;
            }
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
