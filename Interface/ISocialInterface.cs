using SSTUScheduleBot.Models;

namespace SSTUScheduleBot.Interface
{
    public delegate void Message(long userId, string m, string username);

    public interface ISocialInterface
    {
        event Message OnMessageArrived;

        void Send(User u, string m, string[]? answerButtons);
    }
}