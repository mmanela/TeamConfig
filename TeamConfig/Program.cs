using System;
using System.Diagnostics;
using TeamConfig.PowerShellConfig;
using TeamConfig.Razor;
using TeamConfig.Wrappers;

namespace TeamConfig
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Stopwatch w = new Stopwatch();
            w.Start();
            try
            {
                var configFile = args[0];
                var templateFiles = args[1].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries); ;
                var outputPaths = args[2].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries); ;

                var runner = new ConfigRunner(new FileSystemWrapper(), new PowerShellConfigProvider(), new RazorTemplateGenerator());
                runner.RunConfig(configFile, templateFiles, outputPaths);
            }
            catch(TemplateFileException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error running template");
                Console.WriteLine("\t" + ex.Message);
            }
            catch (TemplateCompileException tce)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error generating template");
                foreach (var e in tce.Errors)
                {
                    Console.WriteLine("\t" + e);
                }
            }


            Console.ResetColor();

            w.Stop();
            Console.WriteLine("Took {0} seconds", w.ElapsedMilliseconds / 1000.0);
        }
    }
}