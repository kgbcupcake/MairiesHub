using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MairiesHub.Models;
using MairiesHub.Services;

namespace MairiesHub.ViewModels;

public partial class WireGuardViewModel : ViewModelBase
{
    private readonly IWireGuardService _wgService;
    private DispatcherTimer? _refreshTimer;

    [ObservableProperty]
    private string _wgStatus = "Unknown";

    [ObservableProperty]
    private int _peerCount;

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<WireGuardPeer> Peers { get; } = new();

    public WireGuardViewModel() : this(new WireGuardService()) { }

    public WireGuardViewModel(IWireGuardService wgService)
    {
        _wgService = wgService;
    }

    public async Task InitAsync()
    {
        await LoadAsync();
        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(8000) };
        _refreshTimer.Tick += async (_, _) => await LoadAsync();
        _refreshTimer.Start();
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        var result = await Task.Run(() => _wgService.GetWireGuardAsync());
        Dispatcher.UIThread.Post(() =>
        {
            Peers.Clear();
            if (result != null)
            {
                WgStatus = result.Status;
                PeerCount = result.PeerCount;
                foreach (var p in result.Peers)
                    Peers.Add(p);
            }
            else
            {
                WgStatus = "offline";
            }
            IsLoading = false;
        });
    }

    public void Dispose()
    {
        _refreshTimer?.Stop();
    }
}
