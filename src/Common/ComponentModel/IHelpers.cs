using System.Text.RegularExpressions;

namespace ComponentModel {
    public interface IHelpers {
        bool CanAcceptWithRegex(string receivedText, Regex matcher, string expectedCommand);
        void Log(string message);
    }
}