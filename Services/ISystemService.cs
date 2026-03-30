using System.Threading.Tasks;
using MairiesHub.Models;

namespace MairiesHub.Services;

public interface ISystemService
{
    Task<SystemStats?> GetSystemStatsAsync();
}
