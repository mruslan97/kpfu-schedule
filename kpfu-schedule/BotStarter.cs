using System;
using System.Configuration;
using System.Threading.Tasks;
using kpfu_schedule.Jobs;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace kpfu_schedule
{
    internal class BotStarter
    {
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient(ConfigurationManager.AppSettings["BotToken"]);

        //private static readonly MessageHandler MessageHandler = new MessageHandler();
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {           
            UpdateScheduler.Start();
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnReceiveError += BotOnReceiveError;
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.TextMessage) return;
            //await Task.Run(() => MessageHandler.SortInputMessage(message));
            await Task.Run(() =>
            {
                var messageHandler = new MessageHandler();
                messageHandler.SortInputMessage(message);
            });
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            _logger.Warn("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}