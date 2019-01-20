using System.IO;
using System.Threading.Tasks;

namespace BotHost.Services
{
    /// <summary> Сервис генерации PDF ответа </summary>
    public interface IPdfGenerator
    {
        Task<MemoryStream> GetPdf(long chatId);
    }
}