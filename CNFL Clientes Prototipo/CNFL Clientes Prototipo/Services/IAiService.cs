using System.Threading.Tasks;

namespace CNFL_Clientes_Prototipo.Services
{
    public interface IAiService
    {
        Task<string> ProcessAsync(string prompt);
    }
}