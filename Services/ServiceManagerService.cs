using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MairiesHub.Http;
using MairiesHub.Models;

namespace MairiesHub.Services;

public class ServiceManagerService : IServiceManagerService
{
    public async Task<List<ServiceInfo>> GetServicesAsync()
    {
        try
        {
            var response = await SharedHttpClient.Instance.Client.GetAsync("/api/services");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var result = JsonSerializer.Deserialize<ServicesResponse>(json);
                return result?.Services ?? new List<ServiceInfo>();
            }
            catch (JsonException)
            {
                return new List<ServiceInfo>();
            }
        }
        catch (HttpRequestException)
        {
            return new List<ServiceInfo>();
        }
        catch (Exception)
        {
            return new List<ServiceInfo>();
        }
    }

    public async Task<ServiceControlResult?> StartServiceAsync(string name)
    {
        try
        {
            var response = await SharedHttpClient.Instance.Client.PostAsync($"/api/services/{name}/start", null);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<ServiceControlResult>(json);
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

    public async Task<ServiceControlResult?> StopServiceAsync(string name)
    {
        try
        {
            var response = await SharedHttpClient.Instance.Client.PostAsync($"/api/services/{name}/stop", null);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                return JsonSerializer.Deserialize<ServiceControlResult>(json);
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
