using System;
using System.Management.Automation;

namespace TeamConfig.PowerShellConfig
{
    [Cmdlet(VerbsData.Import, "Config")]
    public class ImportConfigCommand : Cmdlet
    {
        private PowerShellConfigProvider provider;
        public ImportConfigCommand()
        {
            provider = new PowerShellConfigProvider();
        }
        /// <summary>
        /// Names of processes to be stopped.
        /// </summary>
        private string[] fileNames;

        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string[] Name
        {
            get { return this.fileNames; }
            set { this.fileNames = value; }
        }

        protected override void ProcessRecord()
        {
            foreach (string name in this.fileNames)
            {
                try
                {
                    provider.GetConfigValues(name);
                }
                catch (Exception e)
                {
                    WriteError(new ErrorRecord(e, "UnableToProcessFile", ErrorCategory.ReadError, name));
                }

            }

        }
    }
}