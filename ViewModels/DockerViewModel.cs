using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MairiesHub.Models;
using MairiesHub.Services;

namespace MairiesHub.ViewModels;

public partial class DockerViewModel : ViewModelBase
{
    private readonly IDockerService _dockerService;
    private DispatcherTimer? _refreshTimer;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ObservableCollection<ContainerInfo> Containers { get; } = new();

    public int ContainerCount => Containers.Count;

    public DockerViewModel() : this(new DockerService()) { }

    public DockerViewModel(IDockerService dockerService)
    {
        _dockerService = dockerService;
    }

    public async Task InitAsync()
    {
        await LoadContainersAsync();
        StartRefreshTimer();
    }

    private void StartRefreshTimer()
    {
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(8000)
        };
        _refreshTimer.Tick += async (_, _) => await LoadContainersAsync();
        _refreshTimer.Start();
    }

    private async Task LoadContainersAsync()
    {
        IsLoading = true;
        StatusMessage = "Refreshing...";

        var result = await Task.Run(() => _dockerService.GetContainersAsync());

        Dispatcher.UIThread.Post(() =>
        {
            Containers.Clear();
            if (result != null)
            {
                foreach (var c in result)
                    Containers.Add(c);
                StatusMessage = $"{Containers.Count} containers";
            }
            else
            {
                StatusMessage = "Unable to connect";
            }
            IsLoading = false;
        });
    }

    public void Dispose()
    {
        _refreshTimer?.Stop();
    }
}
