using System.Collections.Generic;
using System.Threading.Tasks;
using MairiesHub.Models;

namespace MairiesHub.Services;

public interface IDockerService
{
    Task<List<ContainerInfo>> GetContainersAsync();
}
