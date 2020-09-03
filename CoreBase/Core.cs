using System;
using System.Threading;
using SSTUScheduleBot.Interface;
using SSTUScheduleBot.Schedule;

namespace SSTUScheduleBot.CoreBase
{
    public class Core
    {
        public static IDebugger Debugger = null!;
        public static IDbWorker DbWorker = null!;
        public static Core      Instance = null!;

        private readonly IUserLogic _userLogic;

        private const int TimerDelay = 15 * 60 * 1000;

        public Core(IDbWorker dbWorker, IUserLogic userLogic, IDebugger debugger)
        {
            Instance = this;

            DbWorker   = dbWorker;
            _userLogic = userLogic;
            Debugger   = debugger;


            Debugger.Write($"{DbWorker.GetType()} started");
            Debugger.Write($"{_userLogic.GetType()} started");

            //Thread timer = new Thread(TimerNotifier);
            //timer.Start();
        }

        public void ReparseSchedule()
        {
            Debugger.Write("Schedule parsing started...");
            ScheduleParser parser = new ScheduleParser();
            Debugger.Write("Saving schedule to db started...");
            DbWorker.ReParseSchedule(parser.Parse());
            Debugger.Write("Save to db: success");
        }

        public void ReparseGroup(int id)
        {
            Debugger.Write("Schedule parsing started...");
            ScheduleParser parser = new ScheduleParser();
            var            groupSchedule = parser.ParseLessons(id).Result;
            Debugger.Write("Schedule parsing successfully");

            DbWorker.ParseGroupSchedule(id, groupSchedule);
        }

        private void TimerNotifier()
        {
            DateTime now;
            var      previousDay = DateTime.Now.Day;

            while (true)
            {
                now = DateTime.Now;

                Debugger.Write($"Timer tick on {now}");

                var scheduleItem = _userLogic.Schedule.Find(s =>
                    s.Time.Hours == now.Hour && s.Time.Minutes == now.Minute && !s.IsPassed);

                if (scheduleItem != null)
                {
                    try
                    {
                        _userLogic.NotifyUsers(now);
                    }
                    catch (Exception e)
                    {
                        Debugger.Write(e);
                        return;
                    }

                    scheduleItem.IsPassed = true;

                    Debugger.Write("Schedule sent");
                }

                if (previousDay != now.Day) _userLogic.Schedule.ForEach(s => s.IsPassed = false);

                previousDay = now.Day;

                Thread.Sleep(TimerDelay);
            }
        }
    }
}