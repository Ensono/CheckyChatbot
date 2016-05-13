using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentModel
{
    public interface IChatbotCommand {
        bool CanAccept(string receivedText, bool wasMentioned);

        int Priority { get; set; }

        void Process(string command, Action<string> messageHandler);
    }
}
