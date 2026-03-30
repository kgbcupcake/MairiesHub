using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MairiesHub.Models;
using MairiesHub.Services;

namespace MairiesHub.ViewModels;

public partial class ServicesViewModel : ViewModelBase
{
    private readonly IServiceManagerService _serviceManager;
    private DispatcherTimer? _refreshTimer;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ObservableCollection<ServiceInfo> Services { get; } = new();

    public ServicesViewModel() : this(new ServiceManagerService()) { }

    public ServicesViewModel(IServiceManagerService serviceManager)
    {
        _serviceManager = serviceManager;
    }

    public async Task InitAsync()
    {
        await LoadServicesAsync();
        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(8000) };
        _refreshTimer.Tick += async (_, _) => await LoadServicesAsync();
        _refreshTimer.Start();
    }

    private async Task LoadServicesAsync()
    {
        IsLoading = true;
        var result = await Task.Run(() => _serviceManager.GetServicesAsync());
        Dispatcher.UIThread.Post(() =>
        {
            Services.Clear();
            if (result != null)
            {
                foreach (var s in result)
                    Services.Add(s);
                StatusMessage = $"{Services.Count} services";
            }
            else
            {
                StatusMessage = "Unable to connect";
            }
            IsLoading = false;
        });
    }

    [RelayCommand]
    private async Task StartService(string name)
    {
        await Task.Run(() => _serviceManager.StartServiceAsync(name));
        await LoadServicesAsync();
    }

    [RelayCommand]
    private async Task StopService(string name)
    {
        await Task.Run(() => _serviceManager.StopServiceAsync(name));
        await LoadServicesAsync();
    }

    public void Dispose()
    {
        _refreshTimer?.Stop();
    }
}
