using System.Collections.Generic;

#nullable disable

namespace SSTUScheduleBot.Models
{
    public partial class ConnectionType
    {
        public ConnectionType()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
