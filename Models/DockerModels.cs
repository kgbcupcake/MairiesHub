using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MairiesHub.Models;

public class ContainerInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("image")]
    public string Image { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("ports")]
    public List<string> Ports { get; set; } = new();

    [JsonPropertyName("health")]
    public string Health { get; set; } = string.Empty;

    [JsonIgnore]
    public string PortsDisplay => Ports.Count > 0 ? string.Join("  ", Ports) : "—";

    [JsonIgnore]
    public bool PortsVisible => Ports.Count > 0;

    [JsonIgnore]
    public string DisplayPort => Ports.Count > 0 ? Ports[0] : string.Empty;

    [JsonIgnore]
    public string HealthLabel => string.IsNullOrEmpty(Health) ? "unknown" : Health.ToLowerInvariant();
}

public class DockerContainersResponse
{
    [JsonPropertyName("containers")]
    public List<ContainerInfo> Containers { get; set; } = new();

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
