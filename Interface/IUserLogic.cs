using System;
using System.Collections.Generic;
using SSTUScheduleBot.Models;

namespace SSTUScheduleBot.Interface
{
    public interface IUserLogic
    {
        public List<TimeItem> Schedule { get; set; }

        void NotifyUsers(in DateTime now);
    }
}