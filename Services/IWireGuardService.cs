using System.Threading.Tasks;
using MairiesHub.Models;

namespace MairiesHub.Services;

public interface IWireGuardService
{
    Task<WireGuardResponse?> GetWireGuardAsync();
}
