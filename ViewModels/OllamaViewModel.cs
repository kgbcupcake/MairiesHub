using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using MairiesHub.Models;
using MairiesHub.Services;

namespace MairiesHub.ViewModels;

public partial class OllamaViewModel : ViewModelBase
{
    private readonly IOllamaService _ollamaService;
    private DispatcherTimer? _refreshTimer;

    [ObservableProperty]
    private string _ollamaStatus = "Unknown";

    [ObservableProperty]
    private int _modelCount;

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<OllamaModel> Models { get; } = new();

    public OllamaViewModel() : this(new OllamaService()) { }

    public OllamaViewModel(IOllamaService ollamaService)
    {
        _ollamaService = ollamaService;
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
        var result = await Task.Run(() => _ollamaService.GetOllamaAsync());
        Dispatcher.UIThread.Post(() =>
        {
            Models.Clear();
            if (result != null)
            {
                OllamaStatus = result.Status;
                ModelCount = result.ModelCount;
                foreach (var m in result.Models)
                    Models.Add(m);
            }
            else
            {
                OllamaStatus = "offline";
            }
            IsLoading = false;
        });
    }

    public void Dispose()
    {
        _refreshTimer?.Stop();
    }
}
