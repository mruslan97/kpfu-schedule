using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BotHost.Commands.CommandsArgs;
using BotHost.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotHost.Commands
{
    public class SetupGroupCommand : CommandBase<DefaultCommandArgs>
    {
        private readonly UsersContext _usersContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SetupGroupCommand> _logger;

        public SetupGroupCommand(UsersContext usersContext, IHttpClientFactory httpClientFactory,
            ILogger<SetupGroupCommand> logger) : base("group")
        {
            _usersContext = usersContext;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        protected override bool CanHandleCommand(Update update)
        {
            if (!base.CanHandleCommand(update)) return update.Message.Text.ToLowerInvariant().Contains("-");
            return true;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(Update update, DefaultCommandArgs args)
        {
            var group = update.Message.Text;
            var checkGroup = await CheckGroup(group);
            if (!checkGroup)
            {
                await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id, "Нет данных для этой группы");
                _logger.LogInformation($"Schedule exist, group:{group}, chatId:{update.Message.Chat.Id}");
                return UpdateHandlingResult.Handled;
            }

            //using (_usersContext)
            //{
                var user = await _usersContext.TgUsers.SingleOrDefaultAsync(u => u.ChatId == update.Message.Chat.Id);
                user.Group = group;
                await _usersContext.SaveChangesAsync();
            //}

            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] {new KeyboardButton("На сегодня")},
                new[] {new KeyboardButton("На завтра")},
                new[] {new KeyboardButton("На неделю")},
                new[] {new KeyboardButton("На неделю(pdf)")}
            });
            await Bot.Client.SendTextMessageAsync(update.Message.Chat.Id, $"Группа сохранена.", replyMarkup: keyboard);
            _logger.LogTrace($"user group change:{update.Message.Chat.Id}, new group:{group}");
            return UpdateHandlingResult.Handled;
        }

        private async Task<bool> CheckGroup(string group)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var bytes = await httpClient.GetByteArrayAsync(
                $"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
            var htmlPage = encoding.GetString(bytes, 0, bytes.Length);
            return !htmlPage.Contains("Расписание не найдено");
        }
    }
}