using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MairiesHub.Http;
using MairiesHub.Models;

namespace MairiesHub.Services;

public class WireGuardService : IWireGuardService
{
    public async Task<WireGuardResponse?> GetWireGuardAsync()
    {
        try
        {
            var response = await SharedHttpClient.Instance.Client.GetAsync("/api/wireguard");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<WireGuardResponse>(json);
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
