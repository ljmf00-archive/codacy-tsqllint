using System;
using TSQLLint.Common;
using Codacy.Engine.Seed;
using Codacy.Engine.Seed.Results;

namespace Codacy.TSQLLint.Reporters
{
    /// <summary>
    /// Custom console reporter for codacy report scheme.
    /// This is a class needed by the tool to report the violated rules
    /// on TSQL code.
    /// </summary>
    public class CodacyConsoleReporter : IReporter
    {
        /// <summary>
        /// Report to stdout a message.
        /// </summary>
        /// <param name="message">Message to report</param>
        public virtual void Report(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Summary of total analyzed files and total run time.
        /// </summary>
        /// <param name="timespan">Total run time</param>
        /// <param name="fileCount">number of files analyzed</param>
        public void ReportResults(TimeSpan timespan, int fileCount)
        {
            Logger.Send($"\nLinted {fileCount} files in {timespan.TotalSeconds} seconds");
        }
        
        public void ReportFileResults()
        { }

        /// <summary>
        /// Report a rule violation.
        /// This is called when a rule is violated on the TSQL code.
        /// </summary>
        /// <param name="violation">rule violation model</param>
        public void ReportViolation(IRuleViolation violation)
        {
            ReportViolation(
                new CodacyResult
                {
                    Filename = violation.FileName.Substring(violation.FileName.IndexOf("/", StringComparison.CurrentCulture) + 1),
                    Message = violation.Text,
                    Line = violation.Line,
                    PatternId = violation.RuleName
                });
        }

        /// <summary>
        /// Report a rule violation.
        /// This is called when a rule is violated on the TSQL code.
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="line">line number</param>
        /// <param name="column">index of the line</param>
        /// <param name="severity">rule severity</param>
        /// <param name="ruleName">rule name</param>
        /// <param name="violationText">violation text description</param>
        public void ReportViolation(string fileName, string line, string column, string severity, string ruleName, string violationText)
        {
            CodacyResult result = new CodacyResult
            {
                Filename = fileName.Substring(fileName.IndexOf("/", StringComparison.CurrentCulture) + 1),
                Message = violationText,
                Line = long.Parse(line),
                PatternId = ruleName
            };

            ReportViolation(result);
        }
        
        /// <summary>
        /// Report a rule violation.
        /// This is called when a rule is violated on the TSQL code.
        /// </summary>
        /// <param name="result">codacy result model</param>
        public void ReportViolation(CodacyResult result)
        {
            Report(result.ToString());
        }
    }
}