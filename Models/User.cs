using System;
using System.Collections.Generic;

#nullable disable

namespace SSTUScheduleBot.Models
{
    public partial class User
    {
        public User()
        {
            Requests = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public long InnerId { get; set; }
        public int ConnectionType { get; set; }
        public int? GroupId { get; set; }
        public int State { get; set; }

        public virtual ConnectionType ConnectionTypeNavigation { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}
