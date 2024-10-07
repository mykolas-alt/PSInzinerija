using PSInzinerija1.Enums;
using PSInzinerija1.Models;

using System.Threading.Tasks;

namespace PSInzinerija1.Services
{
    public interface IGameRulesAPIService
    {
        Task<string?> GetGameRulesAsync();
    }
}