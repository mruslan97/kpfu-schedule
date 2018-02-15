using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using kpfu_schedule.Models;
using NLog;
using SelectPdf;

namespace kpfu_schedule.Tools
{
    public class Cache
    {
        private readonly HtmlToImage _converterHtmlToImage = new HtmlToImage();
        private readonly HtmlParser _htmlParser = new HtmlParser();
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly HttpClient _httpClient = new HttpClient();

        public async void Update()
        {
            _logger.Trace("Start cache update");
            var di = new DirectoryInfo("tmpPng");
            foreach (var file in di.GetFiles())
                file.Delete();
            var groups = new List<string>();
            using (var db = new TgUsersContext())
            {
                groups = db.Users.Select(u => u.Group).Where(g => g != null).Distinct().ToList();
                _logger.Trace("Downloaded groups");
            }
            var day = Convert.ToInt32(DateTime.Today.DayOfWeek);

            foreach (var group in groups)
                await UpdateGroup(group, day);
            _logger.Trace("Cache update successful");
        }

        private async Task UpdateGroup(string group, int day)
        {
            var htmlPage =
                await _httpClient.GetStringAsync($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            UpdateWeek(htmlPage, group);
            _converterHtmlToImage.WebPageWidth = 600;
            if (day != 0)
                UpdateToday(htmlPage, group, day);
            day += 1;
            if (day != 7)
                UpdateTomorrow(htmlPage, group, day);
        }

        private void UpdateToday(string htmlPage, string group, int day)
        {
            var htmlToday = _htmlParser.ParseDay(htmlPage, day);
            var imageToday = _converterHtmlToImage.ConvertHtmlString(htmlToday);
            imageToday.Save($"tmpPng/{group}{true}.png", ImageFormat.Png);
            imageToday.Dispose();
        }

        private void UpdateTomorrow(string htmlPage, string group, int day)
        {
            var htmlTomorrow = _htmlParser.ParseDay(htmlPage, day);
            var imageTomorrow = _converterHtmlToImage.ConvertHtmlString(htmlTomorrow);
            imageTomorrow.Save($"tmpPng/{group}{false}.png", ImageFormat.Png);
            imageTomorrow.Dispose();
        }

        private void UpdateWeek(string htmlPage, string group)
        {
            var imageWeek = _converterHtmlToImage.ConvertHtmlString(htmlPage);
            imageWeek.Save($"tmpPng/{group}week.png", ImageFormat.Png);
            imageWeek.Dispose();
        }
    }
}