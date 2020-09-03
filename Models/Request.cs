using System;
using System.Collections.Generic;

#nullable disable

namespace SSTUScheduleBot.Models
{
    public partial class Request
    {
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public int Request1 { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
