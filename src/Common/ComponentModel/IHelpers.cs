using System.Text.RegularExpressions;

namespace ComponentModel {
    public interface IHelpers {
        bool CanAcceptWithRegex(string receivedText, Regex matcher, string expectedCommand);
        string GetBaseName(string filename);
        string Mask(string input, double coverage = 0.7d);
    }
}