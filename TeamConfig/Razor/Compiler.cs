using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Razor;
using Microsoft.CSharp;

namespace TeamConfig.Razor
{
    public class Compiler
    {
        private static readonly Regex PageDirectivePattern = new Regex(@"@(?:model|using) (?<Namespace>[\w\.]+)\b");

        public static Assembly Compile(RazorTemplateEntry razorTemplateEntry, Type baseType)
        {
            return Compile(new[] { razorTemplateEntry }, baseType);
        }

        public static Assembly Compile(IEnumerable<RazorTemplateEntry> entries, Type baseType)
        {
            var builder = new StringBuilder();
            var codeProvider = new CSharpCodeProvider();
            using (var writer = new StringWriter(builder))
            {
                foreach (var razorTemplateEntry in entries)
                {
                    var generatorResults = GenerateCode(razorTemplateEntry, baseType);
                    codeProvider.GenerateCodeFromCompileUnit(generatorResults.GeneratedCode, writer, new CodeGeneratorOptions());
                }
            }

            var result = codeProvider.CompileAssemblyFromSource(BuildCompilerParameters(), new[] { builder.ToString() });
            if (result.Errors != null && result.Errors.Count > 0)
            {
                var code = builder.ToString();
                throw new TemplateCompileException(result.Errors, code);
            }

            return result.CompiledAssembly;
        }

        private static GeneratorResults GenerateCode(RazorTemplateEntry entry, Type baseType)
        {
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            host.DefaultBaseClass = baseType.FullName;
            host.DefaultNamespace = "TeamConfig.Razor.Template";
            host.DefaultClassName = entry.TemplateName + "Template";
            host.NamespaceImports.Add("System");

            //filter out page directives and add them as namespace
            string templateString = entry.TemplateString;
            foreach (Match match in PageDirectivePattern.Matches(templateString))
            {
                string usedNamespace = match.Groups["Namespace"].Value;
                templateString = templateString.Replace(match.Value, string.Empty);
                if (usedNamespace.StartsWith("using"))
                {
                    host.NamespaceImports.Add(usedNamespace);
                }
            }

            GeneratorResults razorResult = null;

            using (TextReader reader = new StringReader(templateString))
            {
                var templateEngine = new RazorTemplateEngine(host);
                razorResult = templateEngine.GenerateCode(reader);
            }
            return razorResult;
        }

        private static CompilerParameters BuildCompilerParameters()
        {
            var @params = new CompilerParameters();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.ManifestModule.Name != "<In Memory Module>")
                    @params.ReferencedAssemblies.Add(assembly.Location);
            }
            var dynamicAssembly = typeof (Microsoft.CSharp.RuntimeBinder.Binder).Assembly.Location;
            @params.ReferencedAssemblies.Add(dynamicAssembly);
            @params.GenerateInMemory = true;
            @params.IncludeDebugInformation = false;
            @params.GenerateExecutable = false;
            @params.CompilerOptions = "/target:library /optimize";
            return @params;
        }
    }
}