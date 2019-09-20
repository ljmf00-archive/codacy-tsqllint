using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TSQLLint.Common;
using TSQLLint.Core.Interfaces;
using TSQLLint.Infrastructure.Configuration;
using TSQLLint.Infrastructure.Parser;
using TSQLLint.Infrastructure.Plugins;
using TSQLLint.Infrastructure.Reporters;
using Codacy.Engine.Seed;
using Codacy.Engine.Seed.Results;
using Codacy.TSQLLint.Configuration;
using Codacy.TSQLLint.Reporters;
using Newtonsoft.Json;

namespace Codacy.TSQLLint
{
    /// <summary>
    /// Tool integration for TSQLLint using the Seed class.
    /// </summary>
    public class CodeAnalyzer : Engine.Seed.CodeAnalyzer
    {
        private readonly IConfigReader configReader;
        private readonly IReporter reporter;
        private readonly IConsoleTimer timer;

        private IPluginHandler pluginHandler;
        private ISqlFileProcessor fileProcessor;
        
        private const string sqlExtension = ".sql";
        private const string defaultTSQLLintConfiguration = ".tsqllintrc";

        private readonly string tmpTSQLLintPath;

        /// <summary>
        /// Tool integration constructor.
        /// This will prepare everything needed for the tool to work with codacy
        /// integration, using the Seed.
        /// </summary>
        public CodeAnalyzer() : base(sqlExtension)
        {
            this.timer = new ConsoleTimer();
            this.reporter = new CodacyConsoleReporter();
            this.configReader = new ConfigReader(reporter);
            
            // create temporary directory
            var tmpTSQLLintFolder = Path.Combine(Path.GetTempPath(), "tsqllint_" + Guid.NewGuid());
            Directory.CreateDirectory(tmpTSQLLintFolder);

            this.tmpTSQLLintPath = Path.Combine(tmpTSQLLintFolder, defaultTSQLLintConfiguration);

            var tsqllintConfig = new TSQLLintConfiguration();
            
            if (!(PatternIds is null) && PatternIds.Any())
            {
                tsqllintConfig.Rules = new Dictionary<string, string>();
                foreach (var pattern in CurrentTool.Patterns)
                {
                    tsqllintConfig.Rules.Add(pattern.PatternId, "error");
                }

                foreach (var unusedPattern in Patterns.Patterns.Select(p => p.PatternId)
                    .Except(CurrentTool.Patterns.Select(p => p.PatternId)))
                {
                    tsqllintConfig.Rules.Add(unusedPattern, "off");
                }
            } else if (File.Exists(defaultTSQLLintConfiguration))
            {
                var tsqlliteJSON = File.ReadAllText(defaultTSQLLintConfiguration);
                var currentTSQLLintConfig = JsonConvert.DeserializeObject<TSQLLintConfiguration>(tsqlliteJSON);
                tsqllintConfig.Rules = currentTSQLLintConfig.Rules;
                tsqllintConfig.CompatibilityLevel = currentTSQLLintConfig.CompatibilityLevel;
            }
            else
            {
                tsqllintConfig.Rules = new Dictionary<string, string>();
                foreach (var pattern in Patterns.Patterns.Select(p => p.PatternId))
                {
                    tsqllintConfig.Rules.Add(pattern, "error");
                }
            }

            if (tsqllintConfig.CompatibilityLevel is null)
            {
                tsqllintConfig.CompatibilityLevel = 120;
            }
            
            File.WriteAllText(tmpTSQLLintPath, tsqllintConfig.ToString());
        }

        /// <summary>
        /// Free temporary resources created on object construction.
        /// </summary>
        ~CodeAnalyzer()
        {
            // delete created temporary directory
            Directory.Delete(Path.GetDirectoryName(tmpTSQLLintPath));
        }

        /// <summary>
        /// Run analyze task
        /// </summary>
        /// <param name="cancellationToken">task cancellation token</param>
        /// <returns>Task of the tool running</returns>
        protected override async Task Analyze(CancellationToken cancellationToken)
        {
            await Task.Run(RunTool, cancellationToken);
        }


        /// <summary>
        /// Run the actual tool.
        /// This will call the rule processors of the tool, handle the available plugins
        /// and analyze every file.
        /// </summary>
        private void RunTool()
        {
            timer.Start();

            configReader.LoadConfig(tmpTSQLLintPath);
            
            // tool write to Environment.ExitCode when occur an error
            if (Environment.ExitCode != 0)
            {
                Console.WriteLine("Unexpected error. Check your config file!");
                Environment.Exit(1);
            }

            var fragmentBuilder = new FragmentBuilder(configReader.CompatabilityLevel);
            var ruleVisitorBuilder = new RuleVisitorBuilder(configReader, this.reporter);
            var ruleVisitor = new SqlRuleVisitor(ruleVisitorBuilder, fragmentBuilder, reporter);
            pluginHandler = new PluginHandler(reporter);
            fileProcessor = new SqlFileProcessor(ruleVisitor, pluginHandler, reporter, new FileSystem());

            pluginHandler.ProcessPaths(configReader.GetPlugins());
            
            // tool write to Environment.ExitCode when occur an error
            if (Environment.ExitCode != 0)
            {
                Console.WriteLine("Unexpected error. Probably some plugin not loaded properly!");
                Environment.Exit(1);
            }

            Parallel.ForEach(Config.Files, (file) =>
            {
                try
                {
                    fileProcessor.ProcessPath(DefaultSourceFolder + file);
                } catch (Exception e)
                {
                    Logger.Send(e);

                    Console.WriteLine(new CodacyResult
                    {
                        Filename = file,
                        Message = "could not parse the file"
                    });
                }
            });

            if (fileProcessor.FileCount > 0)
            {
                reporter.ReportResults(timer.Stop(), fileProcessor.FileCount);
            }
        }
    }
}