using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BotHost.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelectPdf;
using Telegram.Bot.Types.InputFiles;

namespace BotHost.Tools
{
    public class ImageGenerator
    {
        private readonly HtmlToImage _converterHtmlToImage;
        private readonly HtmlParser _htmlParser;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ImageGenerator> _logger;
        private readonly string _directory;
        private readonly UsersContext _usersContext;

        public ImageGenerator(HtmlParser htmlParser, HtmlToImage htmlToImage, UsersContext usersContext,
            IHttpClientFactory httpClientFactory, ILogger<ImageGenerator> logger)
        {
            _httpClientFactory = httpClientFactory;
            _converterHtmlToImage = htmlToImage;
            _htmlParser = htmlParser;
            _usersContext = usersContext;
            _logger = logger;
            _directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/tmpPng";
        }

        public async Task<MemoryStream> GetDay(string group, bool isToday)
        {
            try
            {
                var day = Convert.ToInt32(DateTime.Today.DayOfWeek);
                if (day == 6 && !isToday || day == 0 && isToday) return null;
                day = isToday ? day : day + 1;
                if (!File.Exists($"{_directory}/{group}{isToday}.png"))
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var bytes = await httpClient.GetByteArrayAsync(
                        $"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
                    var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                    var htmlPage = encoding.GetString(bytes, 0, bytes.Length);
                    var htmlDocument = _htmlParser.ParseDay(htmlPage, day);
                    _converterHtmlToImage.WebPageWidth = 600;
                    var image = _converterHtmlToImage.ConvertHtmlString(htmlDocument);
                    image.Save($"{_directory}/{group}{isToday}.png", ImageFormat.Png);
                    image.Dispose();
                }

                return new MemoryStream(File.ReadAllBytes($"{_directory}/{group}{isToday}.png"));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{e.Message}");
                throw;
            }
        }

        public async Task<MemoryStream> GetWeek(string group)
        {
            if (!File.Exists($"{_directory}/{group}week.png"))
            {
                var httpClient = _httpClientFactory.CreateClient();
                var bytes = await httpClient.GetByteArrayAsync(
                    $"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                var htmlPage = encoding.GetString(bytes, 0, bytes.Length);
                var htmlDocument = _htmlParser.ParseWeek(htmlPage);
                var image = _converterHtmlToImage.ConvertHtmlString(htmlDocument);
                image.Save($"{_directory}/{group}week.png", ImageFormat.Png);
                image.Dispose();
            }

            return new MemoryStream(File.ReadAllBytes($"{_directory}/{group}week.png"));
            //return new InputOnlineFile(fs, "Расписание.png");
        }
    }
}