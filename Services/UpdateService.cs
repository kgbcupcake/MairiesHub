using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MairiesHub.Services;

public partial class UpdateService : ObservableObject
{
    private const string GitHubApiBase = "https://api.github.com/repos/kgbcupcake/MairiesHub";

    private static readonly HttpClient Http = new()
    {
        DefaultRequestHeaders =
        {
            { "User-Agent", $"MairiesHub/{AppVersion.Current}" },
            { "Accept", "application/vnd.github+json" }
        }
    };

    private DispatcherTimer? _timer;
    private GitHubRelease? _latestRelease;

    [ObservableProperty]
    private bool _updateAvailable;

    [ObservableProperty]
    private string _latestVersion = string.Empty;

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private int _downloadProgress;

    public async Task InitAsync()
    {
        await CheckForUpdateAsync();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromHours(4) };
        _timer.Tick += async (_, _) => await CheckForUpdateAsync();
        _timer.Start();
    }

    private async Task CheckForUpdateAsync()
    {
        try
        {
            var release = await Task.Run(() =>
                Http.GetFromJsonAsync<GitHubRelease>($"{GitHubApiBase}/releases/latest"));

            if (release is null) return;

            _latestRelease = release;

            var tagVersion = release.TagName.TrimStart('v');
            LatestVersion = tagVersion;

            if (System.Version.TryParse(tagVersion, out var remote) &&
                System.Version.TryParse(AppVersion.Current, out var current) &&
                remote > current)
            {
                Dispatcher.UIThread.Post(() => UpdateAvailable = true);
            }
        }
        catch
        {
            // Network errors are silent — we'll retry in 4 hours
        }
    }

    [RelayCommand]
    private async Task ApplyUpdate()
    {
        if (_latestRelease is null || IsDownloading) return;

        // Find the .AppImage asset
        string? assetUrl = null;
        foreach (var asset in _latestRelease.Assets)
        {
            if (asset.Name.EndsWith(".AppImage", StringComparison.OrdinalIgnoreCase))
            {
                assetUrl = asset.BrowserDownloadUrl;
                break;
            }
        }

        if (assetUrl is null) return;

        var appImagePath = Environment.GetEnvironmentVariable("APPIMAGE");
        if (string.IsNullOrEmpty(appImagePath)) return;

        var tempPath = Path.Combine(Path.GetTempPath(), "MairiesHub-update.AppImage");

        try
        {
            IsDownloading = true;
            DownloadProgress = 0;

            await Task.Run(async () =>
            {
                using var response = await Http.GetAsync(assetUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var total = response.Content.Headers.ContentLength ?? -1L;
                await using var stream = await response.Content.ReadAsStreamAsync();
                await using var file = File.Create(tempPath);

                var buffer = new byte[81920];
                long downloaded = 0;
                int read;

                while ((read = await stream.ReadAsync(buffer)) > 0)
                {
                    await file.WriteAsync(buffer.AsMemory(0, read));
                    downloaded += read;

                    if (total > 0)
                    {
                        var pct = (int)(downloaded * 100 / total);
                        Dispatcher.UIThread.Post(() => DownloadProgress = pct);
                    }
                }
            });

            // Make executable
            Process.Start(new ProcessStartInfo("chmod", $"+x \"{tempPath}\"")
            {
                UseShellExecute = false
            })?.WaitForExit();

            // Replace running AppImage
            File.Move(tempPath, appImagePath, overwrite: true);

            // Relaunch and exit
            Process.Start(new ProcessStartInfo(appImagePath)
            {
                UseShellExecute = true
            });

            Dispatcher.UIThread.Post(() =>
            {
                (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)
                    ?.Shutdown();
            });
        }
        catch
        {
            IsDownloading = false;
            DownloadProgress = 0;

            try { File.Delete(tempPath); } catch { /* cleanup best-effort */ }
        }
    }

    private sealed class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = string.Empty;

        [JsonPropertyName("assets")]
        public GitHubAsset[] Assets { get; set; } = [];
    }

    private sealed class GitHubAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = string.Empty;
    }
}
