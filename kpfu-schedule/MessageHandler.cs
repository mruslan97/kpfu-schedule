using System.Data.Entity;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using kpfu_schedule.Models;
using kpfu_schedule.Tools;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace kpfu_schedule
{
    public class MessageHandler
    {
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient("444905366:AAG9PlFd6ZusE3hPO_sGETGPhzgM_e7roZg");

        private static HttpClient _httpClient;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly PdfGenerator _pdfGenerator;
        private readonly ImageGenerator _imageGenerator;


        public MessageHandler()
        {
            _pdfGenerator = new PdfGenerator();
            _imageGenerator = new ImageGenerator();
            _httpClient = new HttpClient();
        }

        public async void SortInputMessage(Message message)
        {
            _logger.Trace($"message:{message.Text} from chatId:{message.Chat.Id}");
            switch (message.Text.ToLower())
            {
                case "/start":
                    HelloAnswer(message.Chat);
                    break;
                case "/help":
                    HelpAnswer(message.Chat.Id);
                    break;
                case "/keyboard":
                    UpdateKeyboard(message.Chat.Id);
                    break;
                case "смена группы":
                    ChangeGroupAnswer(message.Chat.Id);
                    break;
                case "на сегодня":
                    _imageGenerator.GetDay(message.Chat.Id, true);
                    break;
                case "на завтра":
                    _imageGenerator.GetDay(message.Chat.Id, false);
                    break;
                case "на неделю(pdf)":
                    _pdfGenerator.GetPdf(message.Chat.Id);
                    break;
                case "на неделю":
                    _imageGenerator.GetWeek(message.Chat.Id);
                    break;
                default:
                    var regex = new Regex(@"^\d{2}.?.?-\d{3}$");
                    if (regex.IsMatch(message.Text))
                    {
                        VerificationAnswer(message.Text, message.Chat.Id);
                    }
                    else
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id,
                            "Неверный формат или данная команда отсутствует");
                        _logger.Info($"Unexpected command {message.Text} from chatId:{message.Chat.Id}");
                    }
                    break;
            }
        }

        private async void HelloAnswer(Chat chat)
        {
            await Bot.SendTextMessageAsync(chat.Id,
                $"Привет, {chat.FirstName}! Введи номер своей группы в формате **-***");
            using (var db = new TgUsersContext())
            {
                if (db.Users.Find(chat.Id) != null) return;
                db.Users.Add(new TgUser
                {
                    ChatId = chat.Id,
                    Username = chat.Username,
                    FirstName = chat.FirstName,
                    LastName = chat.LastName
                });
                await db.SaveChangesAsync();
                _logger.Trace($"user {chat.Id} saved in db");
            }
        }

        private async void UpdateKeyboard(long chatId)
        {
            using (var db = new TgUsersContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                if (user?.Group == null)
                {
                    await Bot.SendTextMessageAsync(chatId, "Сначала введи номер группы");
                    return;
                }

            }
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] {new KeyboardButton("На сегодня")},
                new[] {new KeyboardButton("На завтра")},
                new[] {new KeyboardButton("На неделю")},
                new[] {new KeyboardButton("На неделю(pdf)")}
            });
            await Bot.SendTextMessageAsync(chatId,"Новая клавиатура", replyMarkup: keyboard);
        }

        private async void HelpAnswer(long chatId)
        {
            await Bot.SendTextMessageAsync(chatId,
                "Для смены группы просто отправь новый номер. По критическим ошибкам: t.me/ruslan_m97");
        }

        private async void VerificationAnswer(string group, long chatId)
        {
            var checkGroup = await CheckGroup(group);
            if (!checkGroup)
            {
                await Bot.SendTextMessageAsync(chatId, "Нет данных для этой группы");
                _logger.Info($"Schedule exist, group:{group}, chatId:{chatId}");
                return;
            }
            using (var db = new TgUsersContext())
            {
                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
                user.Group = group;
                await db.SaveChangesAsync();
            }
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new[] {new KeyboardButton("На сегодня")},
                new[] {new KeyboardButton("На завтра")},
                new[] {new KeyboardButton("На неделю")},
                new[] {new KeyboardButton("На неделю(pdf)")}
            });
            await Bot.SendTextMessageAsync(chatId, $"Группа сохранена.", replyMarkup: keyboard);
            _logger.Trace($"user group change:{chatId}, new group:{group}");
        }

        private async void ChangeGroupAnswer(long chatId)
        {
            await Bot.SendTextMessageAsync(chatId, "Отправь номер новой группы");
        }

        private async Task<bool> CheckGroup(string group)
        {
            var result = await _httpClient.GetStringAsync($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
            return !result.Contains("Расписание не найдено");
        }
    }
}