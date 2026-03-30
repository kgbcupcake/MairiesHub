using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MairiesHub.Http;
using MairiesHub.Models;

namespace MairiesHub.Services;

public class SystemService : ISystemService
{
    public async Task<SystemStats?> GetSystemStatsAsync()
    {
        try
        {
            var response = await SharedHttpClient.Instance.Client.GetAsync("/api/system");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<SystemStats>(json);
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
