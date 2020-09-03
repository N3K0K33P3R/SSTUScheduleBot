using System.Collections.Generic;

namespace SSTUScheduleBot.Schedule
{
    public class Lesson
    {
        public string?       LectureHall { get; set; }
        public string?       Name        { get; set; }
        public string?       Type        { get; set; }
        public string?       Teacher     { get; set; }
        public int           Order       { get; set; }
        public int           Day         { get; set; }
        public List<Lesson>? Subgroups   { get; set; }
    }
}