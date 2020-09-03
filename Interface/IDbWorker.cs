using System.Collections.Generic;
using SSTUScheduleBot.Models;
using SSTUScheduleBot.Schedule;
using Group = SSTUScheduleBot.Schedule.Group;

namespace SSTUScheduleBot.Interface
{
    public interface IDbWorker
    {
        void ReParseSchedule(List<Group> groups);

        User                  GetUserInfo(long      id);
        void                  AddUser(User          user);
        Models.Group          GetGroupByName(string s);
        void                  Save();
        List<Models.Schedule> GetTodaySchedule(User     user);
        List<Models.Schedule> GetTomorrowSchedule(User  user);
        List<Models.Schedule> GetWeekSchedule(User      user);
        List<Models.Schedule> GetAllSchedule(User       user);
        List<Models.Schedule> GetYesterdaySchedule(User user);
        Models.Schedule?      GetNextLesson(User        user);
        void                  ParseGroupSchedule(int    id, Dictionary<WeekTypes, List<Lesson>> groupSchedule);
    }
}