using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
            if (day != 0)
                groups.ForEach(group => UpdateToday(group, day));
            day += 1;
            if (day != 6)
                groups.ForEach(group => UpdateTomorrow(group, day));
            groups.ForEach(UpdateWeek);
            _logger.Trace("Cache update successful");
        }

        private async void UpdateToday(string group, int day)
        {
            var htmlToday = await _htmlParser.ParseDay(group, day);
            _converterHtmlToImage.WebPageWidth = 600;
            var imageToday = _converterHtmlToImage.ConvertHtmlString(htmlToday);
            imageToday.Save($"tmpPng/{group}{true}.png", ImageFormat.Png);
            imageToday.Dispose();
        }

        private async void UpdateTomorrow(string group, int day)
        {
            var htmlTomorrow = await _htmlParser.ParseDay(group, day);
            _converterHtmlToImage.WebPageWidth = 600;
            var imageTomorrow = _converterHtmlToImage.ConvertHtmlString(htmlTomorrow);
            imageTomorrow.Save($"tmpPng/{group}{false}.png", ImageFormat.Png);
            imageTomorrow.Dispose();
        }

        private void UpdateWeek(string group)
        {
            _converterHtmlToImage.WebPageWidth = 600;
            var imageWeek = _converterHtmlToImage.ConvertUrl(
                $"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            imageWeek.Save($"tmpPng/{group}week.png", ImageFormat.Png);
            imageWeek.Dispose();
        }
    }
}