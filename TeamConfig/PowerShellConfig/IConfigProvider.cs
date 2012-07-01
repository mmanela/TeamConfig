using System.Collections.Generic;

namespace TeamConfig.PowerShellConfig
{
    public interface IConfigProvider
    {
        IDictionary<string,object> GetConfigValues(string configScript);
        void ClearConfigValues();
    }
}