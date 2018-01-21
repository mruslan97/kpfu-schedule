using System;
using System.Threading.Tasks;
using NLog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace kpfu_schedule
{
    internal class Program
    {
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient("444905366:AAG9PlFd6ZusE3hPO_sGETGPhzgM_e7roZg");

        //private static readonly MessageHandler MessageHandler = new MessageHandler();
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            //MessageScheduler.Start();
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