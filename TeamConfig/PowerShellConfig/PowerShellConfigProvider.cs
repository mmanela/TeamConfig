using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace TeamConfig.PowerShellConfig
{
    public class PowerShellConfigProvider : IConfigProvider
    {
        public IDictionary<string, object> GetConfigValues(string filePath)
        {
            var scriptText = File.ReadAllText(filePath);
            var iss = InitialSessionState.CreateDefault();
            var importConfig = new SessionStateCmdletEntry("import-config", typeof (ImportConfigCommand), null);
            iss.Commands.Add(importConfig);

            iss.Providers.Add(new SessionStateProviderEntry("teamConfig", typeof (TeamConfigProvider), ""));
            var rs = RunspaceFactory.CreateRunspace(iss);
            rs.Open();
            rs.SessionStateProxy.Path.SetLocation(Path.GetDirectoryName(filePath));
            rs.SessionStateProxy.LanguageMode = PSLanguageMode.FullLanguage;


            var ps = PowerShell.Create();
            ps.Runspace = rs;
            ps.AddScript(scriptText);
            ps.Invoke();
            rs.Close();

            return new Dictionary<string, object>(TeamConfigProvider.ConfigValues);
        }

        public void ClearConfigValues()
        {
            TeamConfigProvider.ConfigValues.Clear();
        }
    }
}