using System.Threading.Tasks;
using MairiesHub.Models;

namespace MairiesHub.Services;

public interface IOllamaService
{
    Task<OllamaResponse?> GetOllamaAsync();
}
