using System;
using System.Collections.Generic;

#nullable disable

namespace SSTUScheduleBot.Models
{
    public partial class SubGroupLesson
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string Classroom { get; set; }
        public string Type { get; set; }

        public virtual Schedule Course { get; set; }
    }
}
