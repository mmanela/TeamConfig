using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace TeamConfig.Razor
{
    public class RazorTemplateGenerator : ITemplateGenerator
    {
        public void ResetCompiledTemplates()
        {
        }
        
        public string RenderFromFile(IDictionary<string,object> configValues, string templatePath)
        {
            return RenderFromText(configValues, File.ReadAllText(templatePath));
        }

        public string RenderFromText(IDictionary<string, object> configValues, string templateString)
        {
            var props = new DynamicProperties();
            foreach (var configValue in configValues)
            {
                props.Add(configValue.Key,configValue.Value);
            }

            var entry = new RazorTemplateEntry() { TemplateString = templateString, TemplateName = "Rzr" + Guid.NewGuid().ToString("N") };
            var compiledTemplateAssembly = Compiler.Compile(entry, typeof(RazorTemplateBase));

            var template = (RazorTemplateBase)compiledTemplateAssembly.CreateInstance("TeamConfig.Razor.Template." + entry.TemplateName + "Template");
            template.Model = props;
            template.Execute();
            var output = template.Buffer.ToString();
            template.Buffer.Clear();
            return output;
        }

    }
}