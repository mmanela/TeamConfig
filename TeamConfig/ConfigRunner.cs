using System;
using System.Collections.Generic;
using System.IO;
using TeamConfig.PowerShellConfig;
using TeamConfig.Razor;
using TeamConfig.Wrappers;

namespace TeamConfig
{
    public class ConfigRunner
    {
        private readonly IFileSystemWrapper fileSystem;
        private readonly IConfigProvider configProvider;
        private readonly ITemplateGenerator templateGenerator;

        public ConfigRunner(IFileSystemWrapper fileSystem, IConfigProvider configProvider, ITemplateGenerator templateGenerator)
        {
            this.fileSystem = fileSystem;
            this.configProvider = configProvider;
            this.templateGenerator = templateGenerator;
        }

        public void RunConfig(string configPath, IList<string> sourceTemplatePaths, IList<string> destinationTemplatePaths)
        {
            ValidateTemplatePaths(sourceTemplatePaths, destinationTemplatePaths);
            var configFile = GetConfigFilePath(configPath);

            for (int i = 0; i < sourceTemplatePaths.Count; i++)
            {
                var sourceTemplatePath = sourceTemplatePaths[i];
                var destinationTemplatePath = destinationTemplatePaths[i];

                ValidateTemplatePath(sourceTemplatePath);

                var outputPath = GetOutputPath(sourceTemplatePath, destinationTemplatePath);
                var configValues = configProvider.GetConfigValues(configFile);
                var res = templateGenerator.RenderFromFile(configValues, sourceTemplatePath);
                fileSystem.WriteAllText(outputPath, res);
            }

        }

        private string GetOutputPath(string sourceTemplatePath, string destinationTemplatePath)
        {
            string outputPath = null;
            if (fileSystem.FileExists(destinationTemplatePath))
            {
                outputPath = destinationTemplatePath;
            }
            else if(fileSystem.FolderExists(destinationTemplatePath))
            {
                outputPath = Path.Combine(destinationTemplatePath, Path.GetFileName(sourceTemplatePath).Replace(".template", ""));
            }

            return fileSystem.GetFullPath(outputPath);
        }

        private void ValidateTemplatePaths(IList<string> sourceTemplatePaths, IList<string> destinationTemplatePaths)
        {
            if (sourceTemplatePaths.Count != destinationTemplatePaths.Count) 
                throw new ArgumentException("Number of source template paths must match the number of destination template paths");
        }

        private void ValidateTemplatePath(string sourceTemplatePath)
        {
            if (!fileSystem.FileExists(sourceTemplatePath))
            {
                throw new FileNotFoundException("Unable to find template file " + sourceTemplatePath);
            }
        }
                  

        private string GetConfigFilePath(string configPath)
        {
            const string fileFormat = "{0}.ps1";
            string foundFilePath = null;
            if (fileSystem.FileExists(configPath))
            {
                foundFilePath = configPath;
            }
            else if(fileSystem.FolderExists(configPath))
            {
                var fileName = string.Format(fileFormat, Environment.MachineName);
                var filePath = Path.Combine(configPath,fileName);
                if (fileSystem.FileExists(filePath))
                {
                    foundFilePath = filePath;
                }
            }

            if(string.IsNullOrEmpty(foundFilePath))
            {
                throw new FileNotFoundException("Unable to find config file for " + Environment.MachineName);
            }

            return fileSystem.GetFullPath(foundFilePath);
        }
    }
}