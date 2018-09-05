using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot.Framework;
using Telegram.Bot.Types;

namespace BotHost
{
    public class KpfuScheduleBot : BotBase<KpfuScheduleBot>
    {
        private readonly ILogger<KpfuScheduleBot> logger;
        private UsersContext _usersContext;
        public KpfuScheduleBot(IOptions<BotOptions<KpfuScheduleBot>> botOptions, UsersContext usersContext, ILogger<KpfuScheduleBot> logger = null) : base(botOptions)
        {
            this.logger = logger;
           // _usersContext = usersContext;
        }

        public override Task HandleUnknownUpdate(Update update)
        {
            logger?.LogWarning("Handler not found: {0}", JsonConvert.SerializeObject(update));
            return Task.Run(() => Console.WriteLine($"unexpected message: {update?.Message?.Text} from {update?.Message?.Chat}"));
        }

        public override Task HandleFaultedUpdate(Update update, Exception e)
        {
            logger?.LogError(e, "Faulted update: {0}", JsonConvert.SerializeObject(update.Message));
            return Client.SendTextMessageAsync(
                update.Message.Chat.Id,
                "500 INTERNAL BOT ERROR (я сломался 😭)");
        }
    }
}
