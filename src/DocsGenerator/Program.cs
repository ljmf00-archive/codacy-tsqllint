using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TSQLLint.Infrastructure.Rules;
using System.IO;
using Codacy.Engine.Seed.Patterns;
using TSQLLint.Core.Interfaces;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Codacy.TSQLLint.DocsGenerator
{
    using Helpers;
    internal static class Program
    {
        private const string docsFolder = @"docs/";
        private const string descriptionFolder = docsFolder + @"description/";
        private const string rulesDocumentation = @"tsqllint/documentation/rules/";

        public static int Main(string[] args)
        {
            Directory.CreateDirectory(docsFolder);
            Directory.CreateDirectory(descriptionFolder);

            string[] files = System.IO.Directory.GetFiles(rulesDocumentation);
            foreach (string file in files)
            {
                var fileName = System.IO.Path.GetFileName(file);
                var destFile = System.IO.Path.Combine(descriptionFolder, fileName);
                System.IO.File.Copy(file, destFile, true);
            }


            var tsqllintVersion = XDocument.Load(@"tsqllint/source/TSQLLint.Console/TSQLLint.Console.csproj").Root
                .Elements("PropertyGroup")
                .SelectMany(pg => pg.Elements())
                .First(e => e.Name == "Version")
                .Value;

            var patternsFile = new CodacyPatterns
            {
                Name = "TSQLLint",
                Version = tsqllintVersion,
                Patterns = new List<Pattern>()
            };
            
            var descriptions = new CodacyDescription();
            
            var types = Assembly
                .LoadFile(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/TSQLLint.Infrastructure.dll")
                .GetTypes()
                .Where(t => t.Namespace == "TSQLLint.Infrastructure.Rules" &&
                            t.GetInterfaces().Contains(typeof(ISqlRule)) &&
                            t.Name.EndsWith("Rule")
                );

            foreach (Type ruleType in types)
            {
                Object instance = Activator.CreateInstance(ruleType, (Action<string, string, int, int>) ((_, __, ___, ____) => { }));
                var pattern = new Pattern();

                var description = new Description();

                foreach (var prop in ruleType.GetProperties())
                {
                    if (prop.Name == "RULE_NAME")
                    {
                        string patternId = (string) prop.GetValue(instance);

                        pattern.Category = CategoryHelper.ToCategory(patternId);
                        pattern.Level = LevelHelper.ToLevel(patternId);
                        pattern.PatternId = patternId;
                        description.PatternId = patternId;
                    } else if (prop.Name == "RULE_TEXT")
                    {
                        description.Title = (string) prop.GetValue(instance);
                    }
                }
                
                patternsFile.Patterns.Add(pattern);
                descriptions.Add(description);
            }

            File.WriteAllText(docsFolder + @"/patterns.json", patternsFile.ToString(true));
            File.WriteAllText(descriptionFolder + @"/description.json", descriptions.ToString(true));

            return 0;
        }
    }
}
