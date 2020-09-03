using System;

namespace SSTUScheduleBot.Models
{
    public class TimeItem
    {
        public TimeSpan Time     { get; set; }
        public bool     IsPassed { get; set; }
    }
}