using System;
using System.Collections.Generic;

#nullable disable

namespace SSTUScheduleBot.Models
{
    public partial class Group
    {
        public Group()
        {
            Schedules = new HashSet<Schedule>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Formattedname { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
