using System;
using System.Reflection;

namespace Checky.Slack.AlertProxy.Areas.HelpPage.ModelDescriptions {
    public interface IModelDocumentationProvider {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}