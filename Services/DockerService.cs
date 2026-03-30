using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MairiesHub.Http;
using MairiesHub.Models;

namespace MairiesHub.Services;

public class DockerService : IDockerService
{
    public async Task<List<ContainerInfo>> GetContainersAsync()
    {
        try
        {
            var response = await SharedHttpClient.Instance.Client.GetAsync("/api/docker/containers");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var result = JsonSerializer.Deserialize<DockerContainersResponse>(json);
                return result?.Containers ?? new List<ContainerInfo>();
            }
            catch (JsonException)
            {
                return new List<ContainerInfo>();
            }
        }
        catch (HttpRequestException)
        {
            return new List<ContainerInfo>();
        }
        catch (Exception)
        {
            return new List<ContainerInfo>();
        }
    }
}
