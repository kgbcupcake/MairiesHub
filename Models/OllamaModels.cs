using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MairiesHub.Models;

public class OllamaModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("size_gb")]
    public double SizeGb { get; set; }
}

public class OllamaResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("model_count")]
    public int ModelCount { get; set; }

    [JsonPropertyName("models")]
    public List<OllamaModel> Models { get; set; } = new();
}
