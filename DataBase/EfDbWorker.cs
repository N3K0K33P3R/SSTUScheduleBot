using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SSTUScheduleBot.Interface;
using SSTUScheduleBot.Models;
using SSTUScheduleBot.Schedule;
using Group = SSTUScheduleBot.Models.Group;

namespace SSTUScheduleBot.DataBase
{
    public class EfDbWorker : IDbWorker
    {
        private DbDataContext _dataContext;

        public EfDbWorker(Config config)
        {
            _dataContext = new DbDataContext(config);
        }

        public void ReParseSchedule(List<Schedule.Group> groups)
        {
            var batchSize = 10;
            var current   = 0;

            foreach (var @group in groups.Distinct(new GroupsComparer()))
            {
                _dataContext.Groups.Add(new Group
                {
                    Id   = group.Id,
                    Name = group.Name
                });

                if (group.Lessons == null)
                {
                    continue;
                }

                foreach (var week in group.Lessons)
                {
                    foreach (var lesson in week.Value)
                    {
                        var schedule = new Models.Schedule();

                        schedule.Day        = lesson.Day;
                        schedule.Name       = lesson.Name;
                        schedule.GroupId    = group.Id;
                        schedule.IsEvenWeek = week.Key == WeekTypes.Even;
                        schedule.Order      = lesson.Order;

                        List<SubGroupLesson> subGroupLessons = null!;

                        if (lesson.Subgroups == null)
                        {
                            schedule.Classroom  = lesson.LectureHall;
                            schedule.Teacher    = lesson.Teacher;
                            schedule.Type       = lesson.Type;
                            schedule.IsSubgroup = false;
                        }
                        else
                        {
                            schedule.IsSubgroup = true;

                            subGroupLessons = lesson.Subgroups.Select(subgroup => new SubGroupLesson
                            {
                                Classroom = subgroup.LectureHall, Teacher = subgroup.Teacher, Name = subgroup.Name,
                                Course    = schedule
                            }).ToList();
                        }

                        _dataContext.Schedules.Add(schedule);

                        if (subGroupLessons != null)
                        {
                            _dataContext.SubGroupLessons.AddRange(subGroupLessons);
                        }

                        current++;

                        if (current >= batchSize)
                        {
                            _dataContext.SaveChanges();
                            current = 0;
                        }
                    }
                }
            }
        }

        public User GetUserInfo(long id)
        {
            return _dataContext.Users.FirstOrDefault(u => u.InnerId == id);
        }

        public void AddUser(User user)
        {
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();
        }

        public Group GetGroupByName(string s)
        {
            s = GetDefaultString(s);
            return _dataContext.Groups.FirstOrDefault(g => g.Formattedname == s);
        }

        public List<Models.Schedule> GetTodaySchedule(User user)
        {
            var currentDay = (int) DateTime.Now.DayOfWeek - 1;

            return GetScheduleOnDay(currentDay, IsEvenWeek(DateTime.Now), user.GroupId!.Value);
        }

        public List<Models.Schedule> GetTomorrowSchedule(User user)
        {
            var dateTimeNow = DateTime.Now;

            var dateTimeTomorrow = dateTimeNow.AddDays(1);
            var nextDay          = (int) dateTimeTomorrow.DayOfWeek - 1;

            return GetScheduleOnDay(nextDay, IsEvenWeek(dateTimeTomorrow), user.GroupId!.Value);
        }

        public List<Models.Schedule> GetWeekSchedule(User user)
        {
            var isEven = IsEvenWeek(DateTime.Now);

            return _dataContext.Schedules.Where(s => s.IsEvenWeek == isEven && s.GroupId == user.GroupId)
                .OrderBy(s => s.Order)
                .Include(s => s.SubGroupLessons)
                .ToList();
        }

        public List<Models.Schedule> GetAllSchedule(User user)
        {
            throw new NotImplementedException();
        }

        public List<Models.Schedule> GetYesterdaySchedule(User user)
        {
            var dateTimeNow = DateTime.Now;

            var dateTimeYesterday = dateTimeNow.AddDays(-1);
            var nextDay           = (int) dateTimeYesterday.DayOfWeek - 1;

            return GetScheduleOnDay(nextDay, IsEvenWeek(dateTimeYesterday), user.GroupId!.Value);
        }

        public Models.Schedule? GetNextLesson(User user)
        {
            var order = GetNextLessonId(DateTime.Now);

            return GetTodaySchedule(user).Find(s => s.Order == order);
        }

        public void ParseGroupSchedule(int id, Dictionary<WeekTypes, List<Lesson>> groupSchedule)
        {
            _dataContext.Schedules.RemoveRange(_dataContext.Schedules.Where(s => s.GroupId == id));

            foreach (var (key, value) in groupSchedule)
            {
                foreach (var lesson in value)
                {
                    var schedule = new Models.Schedule();

                    schedule.Day        = lesson.Day;
                    schedule.Name       = lesson.Name;
                    schedule.GroupId    = id;
                    schedule.IsEvenWeek = key == WeekTypes.Even;
                    schedule.Order      = lesson.Order;

                    List<SubGroupLesson> subGroupLessons = null!;

                    if (lesson.Subgroups == null)
                    {
                        schedule.Classroom  = lesson.LectureHall;
                        schedule.Teacher    = lesson.Teacher;
                        schedule.Type       = lesson.Type;
                        schedule.IsSubgroup = false;
                    }
                    else
                    {
                        schedule.IsSubgroup = true;

                        subGroupLessons = lesson.Subgroups.Select(subgroup => new SubGroupLesson
                        {
                            Classroom = subgroup.LectureHall, Teacher = subgroup.Teacher, Name = subgroup.Name,
                            Course    = schedule
                        }).ToList();
                    }

                    _dataContext.Schedules.Add(schedule);

                    if (subGroupLessons != null)
                    {
                        _dataContext.SubGroupLessons.AddRange(subGroupLessons);
                    }
                }
            }

            _dataContext.SaveChanges();
        }

        public void LogRequest(User user, DateTime dateTime, ActionTypes actionType)
        {
            _dataContext.Requests.Add(new Request()
            {
                Datetime = dateTime,
                User = user,
                Request1 = (int)actionType
            });

            _dataContext.SaveChanges();
        }

        public void Save()
        {
            _dataContext.SaveChanges();
        }


        private List<Models.Schedule> GetScheduleOnDay(int day, bool isEven, int groupId)
        {
            return _dataContext.Schedules
                .Where(s => s.IsEvenWeek == isEven && s.Day == day && s.GroupId == groupId)
                .OrderBy(s => s.Order)
                .Include(s => s.SubGroupLessons)
                .ToList();
        }

        private bool IsEvenWeek(DateTime date)
        {
            CultureInfo myCi  = CultureInfo.CurrentCulture;
            Calendar    myCal = myCi.Calendar;

            return myCal.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) % 2 ==
                   0;
        }

        private int GetNextLessonId(DateTime now)
        {
            var timespan = new TimeSpan(now.Hour, now.Minute, now.Second);

            var list = Vault.LessonTimeSchedule.ToList();
            list.Add(timespan);

            return list.OrderBy(x => x).ToList().FindIndex(t => t == timespan);
        }

        private string GetDefaultString(string s)
        {
            return s.Replace(" ", "")
                .Replace("-", "")
                .ToLower();
        }

        // ReSharper disable once UnusedMember.Local
        private void ConvertGroups()
        {
            var groups = _dataContext.Groups.ToList();

            foreach (var @group in groups)
            {
                group.Formattedname = GetDefaultString(group.Name);
            }

            _dataContext.SaveChanges();
        }
    }
}