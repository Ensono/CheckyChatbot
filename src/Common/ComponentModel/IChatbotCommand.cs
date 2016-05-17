using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentModel {
    public interface IChatbotCommand {
        bool CanAccept(string receivedText, bool wasMentioned, bool isDirectMessage);

        int Priority { get; }

        Task Process(string command, string user, Func<string, Task> responseHandler);

        string HelpText { get; }

        string Example { get; }
    }
}