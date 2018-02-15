using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        public void Update()
        {
            Console.WriteLine("started");
            Directory.Delete("tmpPng", true);
            Directory.CreateDirectory("tmpPng");
            _logger.Trace("Starting cache update");
            var groups = new List<string>();
            using (var db = new TgUsersContext())
            {
                groups = db.Users.Select(u => u.Group).Where(g => g != null).Distinct().ToList();
                _logger.Trace("Downloaded groups");
            }
            var day = Convert.ToInt32(DateTime.Today.DayOfWeek);
            groups.ForEach(group => UpdateGroup(group, day));
            _logger.Trace("Cache update successful");
        }

        private async void UpdateGroup(string group, int day)
        {
            var httpClient = new HttpClient();
            var htmlPage = await httpClient.GetStringAsync($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            UpdateWeek(group, htmlPage);
            _converterHtmlToImage.WebPageWidth = 600;
            if (day != 0)
                UpdateToday(htmlPage, group, day);
            day += 1;
            if (day != 6)
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

        private void UpdateWeek(string group, string htmlPage)
        {
            var imageWeek = _converterHtmlToImage.ConvertHtmlString(htmlPage);
            imageWeek.Save($"tmpPng/{group}week.png", ImageFormat.Png);
            imageWeek.Dispose();
        }
    }
}