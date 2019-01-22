using System.IO;
using System.Threading.Tasks;

namespace BotHost.Services
{
    public interface IImageGenerator
    {
        Task<MemoryStream> GetDay(string group, bool isToday);

        Task<MemoryStream> GetWeek(string group);
    }
}