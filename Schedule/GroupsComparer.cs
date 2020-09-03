using System.Collections.Generic;

namespace SSTUScheduleBot.Schedule
{
    public class GroupsComparer : IEqualityComparer<Group>
    {
        public bool Equals(Group x, Group y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Group obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}