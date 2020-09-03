using System;
using SSTUScheduleBot.Interface;
using SSTUScheduleBot.Models;

namespace SSTUScheduleBot.UserLogic
{
    public class UserAction : IUserAction
    {
        public event EventHandler<User>  AddUserEvent         = null!;
        public event EventHandler<User>  UpdateUserEvent      = null!;
        public event Func<long, User>    GetUserInfoAction    = null!;
        public event Func<string, Group> GetGroupByNameAction = null!;

        public void AddUser(User user)
        {
            AddUserEvent(this, user);
        }

        public void UpdateUser(User user)
        {
            UpdateUserEvent(this, user);
        }

        public User GetUserInfo(long id)
        {
            return GetUserInfoAction(id);
        }

        public Group GetGroupByName(string s)
        {
            return GetGroupByNameAction(s);
        }
    }
}