using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSTUScheduleBot.Interface;
using SSTUScheduleBot.Models;
using SSTUScheduleBot.SocialInterfaces;

namespace SSTUScheduleBot.UserLogic
{
    public class DefaultUserLogic : IUserLogic
    {
        private readonly IDbWorker _dbWorker;

        private readonly Dictionary<GetScheduleTypes, string> _getScheduleTypes =
            new Dictionary<GetScheduleTypes, string>()
            {
                {GetScheduleTypes.Next, "Следующая пара"},
                {GetScheduleTypes.Today, "Сегодня"},
                {GetScheduleTypes.Tomorrow, "Завтра"},
                {GetScheduleTypes.Yesterday, "Вчера"}
            };

        public List<TimeItem> Schedule { get; set; }

        public DefaultUserLogic(IDbWorker dbWorker)
        {
            _dbWorker = dbWorker;
            Schedule = new List<TimeItem>()
            {
                new TimeItem() {Time = new TimeSpan(17, 53, 0)}
            };

            TelegramInterface.Instance!.OnMessageArrived += ProcessMessageTg;
        }

        private void ProcessMessageTg(long id, string m, string username)
        {
            var user = _dbWorker.GetUserInfo(id);

            if (user == null)
            {
                user = new User()
                {
                    InnerId        = id,
                    State          = (int) UserStates.New,
                    ConnectionType = 1,
                    Username = username
                };

                _dbWorker.AddUser(user);
            }

            switch ((UserStates) user.State)
            {
                case UserStates.New:
                    TelegramInterface.Instance!.Send(user, "Привет! Введи название своей группы.", null);
                    user.State = (int) UserStates.ChooseGroup;
                    break;
                case UserStates.ChooseGroup:
                    var group = _dbWorker.GetGroupByName(m);
                    if (group == null)
                    {
                        TelegramInterface.Instance!.Send(user,
                            "Не могу найти такую группу. Введи название так же, как в расписании.", null);
                    }
                    else
                    {
                        user.Group = group;
                        user.State = (int) UserStates.Ready;
                        TelegramInterface.Instance!.Send(user,
                            $"Группа {@group.Name} сохранена, теперь можешь получать расписание.",
                            _getScheduleTypes.Values.ToArray());
                    }

                    break;
                case UserStates.Ready:
                    ProcessGetSchedule(user, m);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _dbWorker.Save();
        }

        private void ProcessGetSchedule(User user, string m)
        {
            var type = _getScheduleTypes.FirstOrDefault(x => x.Value == m);
            if (type.Value == null)
            {
                TelegramInterface.Instance!.Send(user,
                    "Неизвестная комманда",
                    _getScheduleTypes.Values.ToArray());
                return;
            }

            switch (type.Key)
            {
                case GetScheduleTypes.Next:
                    var course = _dbWorker.GetNextLesson(user);

                    if (course == null)
                        TelegramInterface.Instance?.Send(user,
                            "Пары закончились",
                            _getScheduleTypes.Values.ToArray());
                    else
                        TelegramInterface.Instance?.Send(user,
                            GenerateCourseText(course),
                            _getScheduleTypes.Values.ToArray());

                    break;
                case GetScheduleTypes.Today:
                    SendScheduleFromFunc(_dbWorker.GetTodaySchedule, user);
                    break;
                case GetScheduleTypes.Tomorrow:
                    SendScheduleFromFunc(_dbWorker.GetTomorrowSchedule, user);
                    break;
                case GetScheduleTypes.Week:
                    SendScheduleFromFunc(_dbWorker.GetWeekSchedule, user);
                    break;
                case GetScheduleTypes.All:
                    SendScheduleFromFunc(_dbWorker.GetAllSchedule, user);
                    break;
                case GetScheduleTypes.Yesterday:
                    SendScheduleFromFunc(_dbWorker.GetYesterdaySchedule, user);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SendZeroLessonsMessage(User user)
        {
            TelegramInterface.Instance?.Send(user,
                "На сегодня пар нет",
                _getScheduleTypes.Values.ToArray());
        }

        public void NotifyUsers(in DateTime now)
        {
        }


        private string GenerateScheduleText(List<Models.Schedule> schedules)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < schedules.Count; i++)
            {
                var course = schedules[i];
                sb.AppendLine(GenerateCourseText(course));

                if (i != schedules.Count - 1)
                {
                    sb.AppendLine("<br>");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private string GenerateCourseText(Models.Schedule course)
        {
            if (course
                .SubGroupLessons.Count == 0)
            {
                return $@"Пара №{course.Order + 1}: _{course.Name}_ _*{course.Type}*_
Преподаватель: *{course.Teacher}*
Аудитория: __{course.Classroom}__";
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Пара №{course.Order + 1}: _{course.Name}_");

                foreach (var courseSubGroupLesson in course.SubGroupLessons)
                    sb.AppendLine($@"Преподаватель: *{courseSubGroupLesson.Teacher}*
Аудитория: __{courseSubGroupLesson.Classroom}__
---");

                return sb.ToString();
            }
        }

        private void SendScheduleFromFunc(Func<User, List<Models.Schedule>> getter, User user)
        {
            var schedule = getter(user);

            if (schedule.Count == 0)
            {
                SendZeroLessonsMessage(user);
                return;
            }

            TelegramInterface.Instance!.Send(user,
                GenerateScheduleText(schedule),
                _getScheduleTypes.Values.ToArray());
        }
    }
}