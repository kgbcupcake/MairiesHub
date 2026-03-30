using System.ComponentModel;
using Avalonia.Controls;
using MairiesHub.Controls;
using MairiesHub.ViewModels;

namespace MairiesHub.Views;

public partial class MainWindow : Window
{
    private SystemViewModel? _systemVm;

    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (_systemVm != null)
            _systemVm.PropertyChanged -= OnSystemVmPropertyChanged;

        if (DataContext is MainWindowViewModel vm)
        {
            _systemVm = vm.SystemVm;
            _systemVm.PropertyChanged += OnSystemVmPropertyChanged;
        }
    }

    private void OnSystemVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SystemViewModel.CpuPercent) && _systemVm != null)
        {
            var heartbeat = this.FindControl<EcgHeartbeatControl>("CpuHeartbeat");
            heartbeat?.AddSample(_systemVm.CpuPercent);
        }
    }
}
