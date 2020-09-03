using System;
using System.IO;
using Newtonsoft.Json;
using SSTUScheduleBot.CoreBase;
using SSTUScheduleBot.DataBase;
using SSTUScheduleBot.Debug;
using SSTUScheduleBot.SocialInterfaces;
using SSTUScheduleBot.UserLogic;

namespace SSTUScheduleBot
{
    internal static class Program
    {
        private static void Main()
        {
            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"));

            TelegramInterface unused     = new TelegramInterface(config);
            var               efDbWorker = new EfDbWorker(config);

            var unused1 = new Core(efDbWorker, new DefaultUserLogic(efDbWorker), new LogFileDebug());

            Console.WriteLine("SSTU Schedule Bot Started");

            Command unused2 = new Command();
        }
    }
}