using System.Text.Json.Serialization;

namespace MairiesHub.Models;

public class SystemStats
{
    [JsonPropertyName("cpu_percent")]
    public double CpuPercent { get; set; }

    [JsonPropertyName("ram_used_gb")]
    public double RamUsedGb { get; set; }

    [JsonPropertyName("ram_total_gb")]
    public double RamTotalGb { get; set; }

    [JsonPropertyName("ram_percent")]
    public double RamPercent { get; set; }

    [JsonPropertyName("disk_used_gb")]
    public double DiskUsedGb { get; set; }

    [JsonPropertyName("disk_total_gb")]
    public double DiskTotalGb { get; set; }

    [JsonPropertyName("disk_percent")]
    public double DiskPercent { get; set; }

    [JsonPropertyName("uptime_hours")]
    public int UptimeHours { get; set; }

    [JsonPropertyName("uptime_days")]
    public int UptimeDays { get; set; }
}
