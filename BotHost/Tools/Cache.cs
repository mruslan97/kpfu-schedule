using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BotHost.Models;
using Microsoft.Extensions.Logging;
using SelectPdf;

namespace BotHost.Tools
{
    public class Cache
    {
        private readonly HtmlToImage _converterHtmlToImage;
        private readonly HtmlToPdf _converterHtmlToPdf;
        private readonly HtmlParser _htmlParser;
        private readonly ILogger<Cache> _logger;
        private readonly string _imagesDirectory;
        private readonly string _pdfDirectory;
        private readonly UsersContext _usersContext;
        private readonly HttpClient _httpClient;

        public Cache(HtmlToImage converterHtmlToImage, HtmlToPdf converterHtmlToPdf, HtmlParser htmlParser,
            UsersContext usersContext, IHttpClientFactory httpClientFactory, ILogger<Cache> logger)
        {
            _converterHtmlToImage = converterHtmlToImage;
            _converterHtmlToPdf = converterHtmlToPdf;
            _htmlParser = htmlParser;
            _usersContext = usersContext;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _imagesDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/tmpPng";
            _pdfDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/tmpPdf";
        }

        public async Task Update()
        {
            _logger.LogInformation("Start cache update");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var directoryInfo = new DirectoryInfo(_imagesDirectory);
            directoryInfo.GetFiles().ToList().ForEach(f => f.Delete());
            directoryInfo = new DirectoryInfo(_pdfDirectory);
            directoryInfo.GetFiles().ToList().ForEach(f => f.Delete());
            var tgGroups = _usersContext.TgUsers.Select(u => u.Group).Where(g => g != null).Distinct().ToList();
            var vkGroups = _usersContext.VkUsers.Select(u => u.Group).Where(g => g != null).Distinct().ToList();
            var groups = vkGroups.Union(tgGroups);
            var day = Convert.ToInt32(DateTime.Today.DayOfWeek);
            foreach (var group in groups)
            {
                try
                {
                    await UpdateGroup(group, day);
                }
                catch (Exception e)
                {
                    _logger.LogError($"group :{group} //{e.Message}");
                }
            }

            stopWatch.Stop();
            _logger.LogInformation($"Cache update successful, time elapsed {stopWatch.Elapsed}");
        }

        private async Task UpdateGroup(string group, int day)
        {
            try
            {
                var bytes = await _httpClient.GetByteArrayAsync(
                    $"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
                var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
                var htmlPage = encoding.GetString(bytes, 0, bytes.Length);
                UpdateWeek(htmlPage, group);
                UpdatePdf(htmlPage, group);
                _converterHtmlToImage.WebPageWidth = 600;
                if (day != 0)
                    UpdateToday(htmlPage, group, day);
                day += 1;
                if (day != 7)
                    UpdateTomorrow(htmlPage, group, day);
            }
            catch (Exception e)
            {
                _logger.LogError($"group :{group} //{e.Message}");
            }
            
        }

        private void UpdateToday(string htmlPage, string group, int day)
        {
            try
            {
                var htmlToday = _htmlParser.ParseDay(htmlPage, day);
                var imageToday = _converterHtmlToImage.ConvertHtmlString(htmlToday);
                imageToday.Save($"{_imagesDirectory}/{group}{true}.png", ImageFormat.Png);
                imageToday.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogError($"group :{group} //{e.Message}");
            }
            
        }

        private void UpdateTomorrow(string htmlPage, string group, int day)
        {
            try
            {
                var htmlTomorrow = _htmlParser.ParseDay(htmlPage, day);
                var imageTomorrow = _converterHtmlToImage.ConvertHtmlString(htmlTomorrow);
                imageTomorrow.Save($"{_imagesDirectory}/{group}{false}.png", ImageFormat.Png);
                imageTomorrow.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogError($"group :{group} //{e.Message}");
            }
            
        }

        private void UpdateWeek(string htmlPage, string group)
        {
            try
            {
                htmlPage = _htmlParser.ParseWeek(htmlPage);
                var imageWeek = _converterHtmlToImage.ConvertHtmlString(htmlPage);
                imageWeek.Save($"{_imagesDirectory}/{group}week.png", ImageFormat.Png);
                imageWeek.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogError($"group :{group} //{e.Message}");
            }
            
        }

        private void UpdatePdf(string htmlPage, string group)
        {
            var pdfDocument = _converterHtmlToPdf.ConvertHtmlString(htmlPage);
            pdfDocument.Save($"{_pdfDirectory}/{group}.pdf");
        }
    }
}