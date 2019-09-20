using System.Diagnostics.CodeAnalysis;

namespace Codacy.TSQLLint
{
    internal static class Program
    {
        [ExcludeFromCodeCoverage]
        public static int Main()
        {
            var analyzer = new CodeAnalyzer();

            analyzer.Run()
                .GetAwaiter().GetResult();

            return 0;
        }
    }
}
