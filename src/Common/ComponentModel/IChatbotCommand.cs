using System;
using System.Threading.Tasks;

namespace ComponentModel {
    public interface IChatbotCommand {
        int Priority { get; }

        string HelpText { get; }

        string Example { get; }
        bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage);

        Task Process(string command, string user, Func<string, Task> responseHandler);
    }
}