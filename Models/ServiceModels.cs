using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MairiesHub.Models;

public enum ServiceHealth
{
    Healthy,
    Degraded,
    Stopped
}

public class ServiceInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("port_label")]
    public string PortLabel { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("running")]
    public bool Running { get; set; }

    [JsonPropertyName("controllable")]
    public bool Controllable { get; set; }

    public ServiceHealth Health => Running ? ServiceHealth.Healthy : ServiceHealth.Stopped;
}

public class ServicesResponse
{
    [JsonPropertyName("services")]
    public List<ServiceInfo> Services { get; set; } = new();
}

public class ServiceControlResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
