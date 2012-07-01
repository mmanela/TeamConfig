using System;

namespace TeamConfig.Razor
{
    public class TemplateFileException : Exception
    {
        public TemplateFileException(string message) : base(message)
        {
        }
    }
}