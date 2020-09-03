using System;
using System.Collections.Generic;

namespace SSTUScheduleBot
{
    public static class Vault
    {
        public static List<TimeSpan> LessonTimeSchedule = new List<TimeSpan>()
        {
            new TimeSpan(8,  0,  0),
            new TimeSpan(9,  45, 0),
            new TimeSpan(11, 30, 0),
            new TimeSpan(13, 40, 0),
            new TimeSpan(15, 20, 0),
            new TimeSpan(17, 00, 0),
            new TimeSpan(18, 40, 0),
            new TimeSpan(20, 20, 0)
        };
    }
}