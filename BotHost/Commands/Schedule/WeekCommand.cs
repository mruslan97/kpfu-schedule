using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Commands.CommandsArgs;
using BotHost.Tools;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;

namespace BotHost.Commands.Schedule
{
    public class WeekCommand : CommandBase<DefaultCommandArgs>
    {
        private ImageGenerator _imageGenerator;
        private readonly DateTime firstEvenWeekStart = new DateTime(2018, 8, 27, 23, 59, 59, DateTimeKind.Utc);
        public WeekCommand(ImageGenerator imageGenerator) : base(name: "week")
        {
            _imageGenerator = imageGenerator;
        }
        protected override bool CanHandleCommand(Update update)
        {
            if (!base.CanHandleCommand(update))
            {
                return update.Message.Text.ToLowerInvariant().Contains("неделю");
            }
            return true;
        }

        public override async Task<UpdateHandlingResult> HandleCommand(Update update, DefaultCommandArgs args)
        {
            var diffDaysCount = (DateTime.UtcNow.Date - firstEvenWeekStart).Days;
            diffDaysCount -= GetCurrentIndexOfDayOfWeekForEuropeanMan();
            var weeksSpent = diffDaysCount / 7;
            var isEven = weeksSpent % 2 == 0;
            var messageText = isEven
                ? $"Это {weeksSpent + 2}-я неделя - четная."
                : $"Это {weeksSpent + 2}-я неделя - нечетная.";
            var image = await _imageGenerator.GetWeek(update.Message.Chat.Id);
            await Bot.Client.SendPhotoAsync(update.Message.Chat.Id, image, messageText);
            return UpdateHandlingResult.Handled;

            int GetCurrentIndexOfDayOfWeekForEuropeanMan()
            {
                var utc = DateTime.UtcNow;
                if (utc.DayOfWeek == DayOfWeek.Sunday)
                    return 6;
                return (int)(utc.DayOfWeek - 1);
            }
        }
    }
}
