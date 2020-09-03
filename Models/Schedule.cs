using System;
using System.Collections.Generic;

#nullable disable

namespace SSTUScheduleBot.Models
{
    public partial class Schedule
    {
        public Schedule()
        {
            SubGroupLessons = new HashSet<SubGroupLesson>();
        }

        public int Id { get; set; }
        public string Classroom { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Teacher { get; set; }
        public int Order { get; set; }
        public int Day { get; set; }
        public bool? IsSubgroup { get; set; }
        public int GroupId { get; set; }
        public bool IsEvenWeek { get; set; }

        public virtual Group Group { get; set; }
        public virtual ICollection<SubGroupLesson> SubGroupLessons { get; set; }
    }
}
