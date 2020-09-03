using System.Collections.Generic;

namespace SSTUScheduleBot.Schedule
{
    public class Group
    {
        public string?                              Name    { get; set; }
        public int                                  Id      { get; set; }
        public Dictionary<WeekTypes, List<Lesson>>? Lessons { get; set; }
    }
}