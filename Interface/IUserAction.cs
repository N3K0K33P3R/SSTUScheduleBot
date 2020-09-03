using System;
using SSTUScheduleBot.Models;

namespace SSTUScheduleBot.Interface
{
    public interface IUserAction
    {
        event EventHandler<User>  AddUserEvent;
        event EventHandler<User>  UpdateUserEvent;
        event Func<long, User>    GetUserInfoAction;
        event Func<string, Group> GetGroupByNameAction;

        void AddUser(User user);

        void UpdateUser(User user);

        User  GetUserInfo(long      id);
        Group GetGroupByName(string s);
    }
}