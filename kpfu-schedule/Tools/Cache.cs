using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
            _logger.Trace("Starting update cache");
            var groups = new List<string>();
            using (var db = new TgUsersContext())
            {
                groups = db.Users.Select(u => u.Group).Distinct().ToList();
                _logger.Trace("Downloaded groups");
            }
            var day = Convert.ToInt32(DateTime.Today.DayOfWeek);
            foreach (var group in groups)
            {
                var htmlToday = _htmlParser.ParseDay(group, day);
                var htmlTomorrow = _htmlParser.ParseDay(group, day + 1);
                _converterHtmlToImage.WebPageWidth = 350;
                var imageToday = _converterHtmlToImage.ConvertHtmlString(htmlToday);
                var imageTomorrow = _converterHtmlToImage.ConvertHtmlString(htmlTomorrow);
                var imageWeek = _converterHtmlToImage.ConvertUrl(
                    $"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
                imageToday.Save($"tmpPng/{group}{true}.png", ImageFormat.Png);
                imageTomorrow.Save($"tmpPng/{group}{false}.png", ImageFormat.Png);
                imageWeek.Save($"tmpPng/{group}week.png", ImageFormat.Png);
                imageToday.Dispose();
                imageTomorrow.Dispose();
                imageWeek.Dispose();
            }
            _logger.Trace("Cache update successful");
        }
    }
}