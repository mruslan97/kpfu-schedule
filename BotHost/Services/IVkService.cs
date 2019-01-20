using System.Threading.Tasks;
using VkNet.BotsLongPollExtension.Model;

namespace BotHost.Services
{
    /// <summary> Сервис работы с API Вконтакте </summary>
    public interface IVkService
    {
        Task GetDay(GroupUpdate groupUpdate, bool isToday);

        Task GetWeek(GroupUpdate groupUpdate);
    }
}