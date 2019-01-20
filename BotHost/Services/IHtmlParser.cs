namespace BotHost.Services
{
    /// <summary> Парсер исходного html  </summary>
    public interface IHtmlParser
    {
        string ParseDay(string htmlPage, int day);

        string ParseWeek(string htmlPage);
    }
}