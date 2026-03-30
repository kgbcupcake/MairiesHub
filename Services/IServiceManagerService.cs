using System.Collections.Generic;
using System.Threading.Tasks;
using MairiesHub.Models;

namespace MairiesHub.Services;

public interface IServiceManagerService
{
    Task<List<ServiceInfo>> GetServicesAsync();
    Task<ServiceControlResult?> StartServiceAsync(string name);
    Task<ServiceControlResult?> StopServiceAsync(string name);
}
