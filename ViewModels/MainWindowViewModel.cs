using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MairiesHub.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private string _currentPageName = "Docker";

    public DockerViewModel DockerVm { get; }
    public ServicesViewModel ServicesVm { get; }
    public WireGuardViewModel WireGuardVm { get; }
    public OllamaViewModel OllamaVm { get; }
    public SystemViewModel SystemVm { get; }

    public MainWindowViewModel()
    {
        DockerVm = new DockerViewModel();
        ServicesVm = new ServicesViewModel();
        WireGuardVm = new WireGuardViewModel();
        OllamaVm = new OllamaViewModel();
        SystemVm = new SystemViewModel();

        _currentPage = DockerVm;

        // Fire and forget init — errors are swallowed in each VM
        _ = DockerVm.InitAsync();
        _ = SystemVm.InitAsync();
    }

    [RelayCommand]
    private void NavigateTo(string page)
    {
        CurrentPageName = page;
        switch (page)
        {
            case "Docker":
                CurrentPage = DockerVm;
                break;
            case "Services":
                CurrentPage = ServicesVm;
                _ = ServicesVm.InitAsync();
                break;
            case "WireGuard":
                CurrentPage = WireGuardVm;
                _ = WireGuardVm.InitAsync();
                break;
            case "Ollama":
                CurrentPage = OllamaVm;
                _ = OllamaVm.InitAsync();
                break;
            default:
                CurrentPage = DockerVm;
                break;
        }
    }
}
