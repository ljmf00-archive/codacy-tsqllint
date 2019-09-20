using System;
using System.IO;
using Codacy.Engine.Seed.Results;
using Codacy.TSQLLint.Reporters;
using TSQLLint.Common;
using Xunit;

namespace Codacy.TSQLLint.Tests
{
    public class CodacyConsoleReporterTests
    {
        private class RuleViolationTest : IRuleViolation
        {
            public int Column { get; set; }
            public string FileName { get; set; }
            public int Line { get; set; }
            public string RuleName { get; set; }
            public RuleViolationSeverity Severity { get; set; }
            public string Text { get; set; }
        }

        private readonly StringWriter testWriter = new StringWriter();
        private readonly CodacyConsoleReporter reporter;

        public CodacyConsoleReporterTests()
        {
            this.reporter = new CodacyConsoleReporter();
            Console.SetOut(testWriter);
        }

        [Fact]
        public void CodacyResultTest()
        {
            var result = new CodacyResult
            {
                Filename = "foo.sql",
                Line = 10,
                Message = "Just a test",
                PatternId = "pattern-test"
            };
            reporter.ReportViolation(result);
            
            Assert.Equal(result.ToString() + Environment.NewLine, testWriter.ToString());
        }
        
        [Fact]
        public void ReportViolationTest()
        {
            reporter.ReportViolation("bar.sql", "10", "1", "error", "pattern-test", "Just a test");
            
            var expectedResult = new CodacyResult
            {
                Filename = "bar.sql",
                Line = 10,
                Message = "Just a test",
                PatternId = "pattern-test"
            }.ToString();

            Assert.Equal(expectedResult + Environment.NewLine, testWriter.ToString());
        }
        
        [Fact]
        public void RuleViolationReportTest()
        {
            reporter.ReportViolation(new RuleViolationTest
            {
                FileName = "foobar.sql",
                Column = 1,
                Line = 15,
                RuleName = "pattern-test-2",
                Severity = RuleViolationSeverity.Error,
                Text = "Just another test"
            });
            
            var expectedResult = new CodacyResult
            {
                Filename = "foobar.sql",
                Line = 15,
                Message = "Just another test",
                PatternId = "pattern-test-2"
            }.ToString();

            Assert.Equal(expectedResult + Environment.NewLine, testWriter.ToString());
        }
        
        [Fact]
        public void ReportTest()
        {
            reporter.Report("just a test");
            
            
            Assert.Equal("just a test" + Environment.NewLine, testWriter.ToString());
        }
    }
}