using System;
using System.Collections.Generic;

namespace TeamConfig.Razor
{
    public interface ITemplateGenerator
    {
        string RenderFromFile(IDictionary<string, object> configValues, string templatePath);
        string RenderFromText(IDictionary<string, object> configValues, string templateString);
    }
}