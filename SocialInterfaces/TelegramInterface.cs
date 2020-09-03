using System;
using System.Collections.Generic;
using System.Linq;
using SSTUScheduleBot.CoreBase;
using SSTUScheduleBot.Interface;
using SSTUScheduleBot.Models;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Message = SSTUScheduleBot.Interface.Message;

namespace SSTUScheduleBot.SocialInterfaces
{
    public class TelegramInterface : ISocialInterface
    {
        public static TelegramInterface? Instance;

        public event Message OnMessageArrived = null!;

        private readonly ITelegramBotClient _botClient;

        public TelegramInterface(Config config)
        {
            Instance = this;

            _botClient = new TelegramBotClient(config.TelegramApiKey);

            var unused = _botClient.GetMeAsync().Result;

            //Core.Debugger.Write("Telegram Interface started.", me.Id, me.Username);

            _botClient.OnMessage += BotClientOnOnMessage;
            _botClient.StartReceiving();
        }

        public async void Send(User u, string m, string[]? answerButtons)
        {
            var safeMessage = ParseMessage(m);

            try
            {
                await _botClient.SendTextMessageAsync(u.InnerId, safeMessage,
                    replyMarkup: answerButtons != null
                        ? new ReplyKeyboardMarkup(ParseKeyboardButtons(answerButtons), true)
                        : null,
                    parseMode:
                    ParseMode.MarkdownV2);
                Core.Debugger.Write($"[TG_BOT] Sent to client {u.InnerId}\\{u.Username}:\n{safeMessage}");
            }
            catch (Exception e)
            {
                Core.Debugger.Write(e);
                Core.Debugger.Write($"Message was:\n{safeMessage}");
            }
        }

        private string ParseMessage(string s)
        {
            var motd = MessageOfTheDay.GetDashes();
            return s
                .Replace("<br>", motd)
                .Replace("!",    "\\!")
                .Replace(".",    "\\.")
                .Replace("(",    "\\(")
                .Replace(")",    "\\)")
                .Replace("-",    "\\-");
        }

        private void BotClientOnOnMessage(object? sender, MessageEventArgs e)
        {
            Core.Debugger.Write($"[TG_BOT][{e.Message.Date}][{e.Message.From.Username}]: {e.Message.Text}");
            OnMessageArrived(e.Message.From.Id, e.Message.Text, e.Message.From.Username);
        }

        private IEnumerable<IEnumerable<KeyboardButton>> ParseKeyboardButtons(string[] buttons)
        {
            KeyboardButton[,] result;
            if (buttons.Length <= 10)
            {
                result = new KeyboardButton[buttons.Length, 1];
                var converted = buttons.Select(b => new KeyboardButton(b)).ToArray();
                for (var i = 0; i < converted.Length; i++) result[i, 0] = converted[i];
            }
            else
            {
                result = new KeyboardButton[3, buttons.Length / 2];
                var converted = buttons.Select(b => new KeyboardButton(b)).ToArray();

                for (var i = 0; i < converted.Length; i++) result[i % 3, i / 3] = converted[i];
            }

            List<List<KeyboardButton>> resultList = new List<List<KeyboardButton>>();
            for (var i = 0; i < result.GetLength(0); i++)
            {
                resultList.Add(new List<KeyboardButton>());
                for (var j = 0; j < result.GetLength(1); j++) resultList[i].Add(result[i, j]);
            }

            return resultList;
        }
    }
}