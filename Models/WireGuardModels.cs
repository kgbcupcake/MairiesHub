using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MairiesHub.Models;

public class WireGuardPeer
{
    [JsonPropertyName("interface")]
    public string? Interface { get; set; }

    [JsonPropertyName("public_key")]
    public string PublicKey { get; set; } = string.Empty;

    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }

    [JsonPropertyName("allowed_ips")]
    public List<string> AllowedIps { get; set; } = new();

    [JsonPropertyName("latest_handshake")]
    public string? LatestHandshake { get; set; }

    [JsonPropertyName("transfer_rx")]
    public long TransferRx { get; set; }

    [JsonPropertyName("transfer_tx")]
    public long TransferTx { get; set; }

    [JsonPropertyName("transfer_raw")]
    public string? TransferRaw { get; set; }

    [JsonIgnore]
    public string AllowedIpsDisplay => AllowedIps.Count > 0 ? string.Join(", ", AllowedIps) : "—";

    [JsonIgnore]
    public string ShortKey => PublicKey.Length > 16 ? PublicKey[..16] + "…" : PublicKey;
}

public class WireGuardResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("interfaces")]
    public List<string>? Interfaces { get; set; }

    [JsonPropertyName("peer_count")]
    public int PeerCount { get; set; }

    [JsonPropertyName("peers")]
    public List<WireGuardPeer> Peers { get; set; } = new();

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
