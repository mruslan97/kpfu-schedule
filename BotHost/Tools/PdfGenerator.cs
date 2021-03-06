﻿using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using BotHost.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SelectPdf;
using Telegram.Bot.Types.InputFiles;

namespace BotHost.Tools
{
    public class PdfGenerator
    {
        private readonly HtmlToPdf _converterHtmlToPdf;
        private readonly UsersContext _usersContext;
        private readonly ILogger<PdfGenerator> _logger;
        private readonly string _directory;

        public PdfGenerator(HtmlToPdf converterHtmlToPdf,
            UsersContext usersContext, ILogger<PdfGenerator> logger)
        {
            _converterHtmlToPdf = converterHtmlToPdf;
            _usersContext = usersContext;
            _logger = logger;
            _directory = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "/tmpPdf";
        }

        public async Task<MemoryStream> GetPdf(long chatId)
        {
            try
            {
                var user = await _usersContext.TgUsers.SingleOrDefaultAsync(u => u.ChatId == chatId);
                var group = user.Group;
                var doc = _converterHtmlToPdf.ConvertUrl($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
                doc.Save($"{_directory}/{group}.pdf");
                var fileStream = new MemoryStream(File.ReadAllBytes($"{_directory}/{group}.pdf"));
                return fileStream;
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message} / chatId : {chatId}");
                throw;
            }
        }
    }
}