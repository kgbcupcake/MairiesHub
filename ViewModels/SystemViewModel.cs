using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MairiesHub.Services;

namespace MairiesHub.ViewModels;

public partial class SystemViewModel : ViewModelBase
{
    private readonly ISystemService _systemService;
    private DispatcherTimer? _refreshTimer;

    [ObservableProperty]
    private double _cpuPercent;

    [ObservableProperty]
    private double _ramPercent;

    [ObservableProperty]
    private double _ramUsedGb;

    [ObservableProperty]
    private double _ramTotalGb;

    [ObservableProperty]
    private double _diskPercent;

    [ObservableProperty]
    private double _diskUsedGb;

    [ObservableProperty]
    private double _diskTotalGb;

    [ObservableProperty]
    private int _uptimeDays;

    [ObservableProperty]
    private int _uptimeHours;

    [ObservableProperty]
    private bool _isConnected;

    public SystemViewModel() : this(new SystemService()) { }

    public SystemViewModel(ISystemService systemService)
    {
        _systemService = systemService;
    }

    public async Task InitAsync()
    {
        await LoadAsync();
        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(5000) };
        _refreshTimer.Tick += async (_, _) => await LoadAsync();
        _refreshTimer.Start();
    }

    private async Task LoadAsync()
    {
        var result = await Task.Run(() => _systemService.GetSystemStatsAsync());
        Dispatcher.UIThread.Post(() =>
        {
            if (result != null)
            {
                CpuPercent = result.CpuPercent;
                RamPercent = result.RamPercent;
                RamUsedGb = result.RamUsedGb;
                RamTotalGb = result.RamTotalGb;
                DiskPercent = result.DiskPercent;
                DiskUsedGb = result.DiskUsedGb;
                DiskTotalGb = result.DiskTotalGb;
                UptimeDays = result.UptimeDays;
                UptimeHours = result.UptimeHours;
                IsConnected = true;
            }
            else
            {
                IsConnected = false;
            }
        });
    }

    public void Dispose()
    {
        _refreshTimer?.Stop();
    }
}
