using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotHost.Vk
{
    public class VkAnswers
    {
        //    private async void HelloAnswer(Chat chat)
        //    {
        //        try
        //        {
        //            await Bot.SendTextMessageAsync(chat.Id,
        //                $"Привет, {chat.FirstName}! Введи номер своей группы в формате **-***");
        //            using (var db = new TgUsersContext())
        //            {
        //                if (db.Users.Find(chat.Id) != null) return;
        //                db.Users.Add(new TgUser
        //                {
        //                    ChatId = chat.Id,
        //                    Username = chat.Username,
        //                    FirstName = chat.FirstName,
        //                    LastName = chat.LastName
        //                });
        //                await db.SaveChangesAsync();
        //                _logger.Trace($"user {chat.Id} saved in db");
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            _logger.Error($"{e.Message} / chatId: {chat.Id}");
        //        }
        //    }

        //    private async void UpdateKeyboard(long chatId)
        //    {
        //        try
        //        {
        //            using (var db = new TgUsersContext())
        //            {
        //                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
        //                if (user?.Group == null)
        //                {
        //                    await Bot.SendTextMessageAsync(chatId, "Сначала введи номер группы");
        //                    return;
        //                }
        //            }

        //            var keyboard = new ReplyKeyboardMarkup(new[]
        //            {
        //                new[] {new KeyboardButton("На сегодня")},
        //                new[] {new KeyboardButton("На завтра")},
        //                new[] {new KeyboardButton("На неделю")},
        //                new[] {new KeyboardButton("На неделю(pdf)")}
        //            });
        //            await Bot.SendTextMessageAsync(chatId, "Новая клавиатура", replyMarkup: keyboard);
        //        }
        //        catch (Exception e)
        //        {
        //            _logger.Error($"keyboard update error {e.Message} / chatId: {chatId}");
        //        }
        //    }

        //    private async void HelpAnswer(long chatId)
        //    {
        //        await Bot.SendTextMessageAsync(chatId,
        //            "Для смены группы просто отправь новый номер. По критическим ошибкам: t.me/ruslan_m97");
        //    }

        //    private async void VerificationAnswer(string group, long chatId)
        //    {
        //        try
        //        {
        //            var checkGroup = await CheckGroup(group);
        //            if (!checkGroup)
        //            {
        //                await Bot.SendTextMessageAsync(chatId, "Нет данных для этой группы");
        //                _logger.Info($"Schedule exist, group:{group}, chatId:{chatId}");
        //                return;
        //            }
        //            using (var db = new TgUsersContext())
        //            {
        //                var user = await db.Users.SingleOrDefaultAsync(u => u.ChatId == chatId);
        //                user.Group = group;
        //                await db.SaveChangesAsync();
        //            }
        //            var keyboard = new ReplyKeyboardMarkup(new[]
        //            {
        //                new[] {new KeyboardButton("На сегодня")},
        //                new[] {new KeyboardButton("На завтра")},
        //                new[] {new KeyboardButton("На неделю")},
        //                new[] {new KeyboardButton("На неделю(pdf)")}
        //            });
        //            await Bot.SendTextMessageAsync(chatId, $"Группа сохранена.", replyMarkup: keyboard);
        //            _logger.Trace($"user group change:{chatId}, new group:{group}");
        //        }
        //        catch (Exception e)
        //        {
        //            _logger.Error($"{e.Message} / chatid: {chatId}");
        //        }
        //    }

        //    private async void ChangeGroupAnswer(long chatId)
        //    {
        //        await Bot.SendTextMessageAsync(chatId, "Отправь номер новой группы");
        //    }

        //    private async Task<bool> CheckGroup(string group)
        //    {
        //        var result = await _httpClient.GetStringAsync($"https://kpfu.ru/week_sheadule_print?p_group_name={group}");
        //        return !result.Contains("Расписание не найдено");
        //    }
    }
}
