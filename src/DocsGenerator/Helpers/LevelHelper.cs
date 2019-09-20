using System;
using Codacy.Engine.Seed.Patterns;

namespace Codacy.TSQLLint.DocsGenerator.Helpers
{
    /// <summary>
    /// Helper to convert to codacy level from a TSQLLint rule
    /// </summary>
    public static class LevelHelper
    {
        /// <summary>
        /// This map a rule to a codacy level.
        /// </summary>
        /// <param name="rule">rule name</param>
        /// <returns>Codacy level</returns>
        /// <exception cref="NotImplementedException">When found a non explicit mapped rule</exception>
        public static Level ToLevel(string rule)
        {
            switch (rule)
            {
                case "conditional-begin-end": return Level.Error;
                case "cross-database-transaction": return Level.Error;
                case "data-compression": return Level.Error;
                case "data-type-length": return Level.Error;
                case "disallow-cursors": return Level.Error;
                case "full-text": return Level.Error;
                case "information-schema": return Level.Error;
                case "keyword-capitalization": return Level.Warning;
                case "linked-server": return Level.Error;
                case "multi-table-alias": return Level.Error;
                case "named-constraint": return Level.Error;
                case "non-sargable": return Level.Error;
                case "object-property": return Level.Error;
                case "print-statement": return Level.Error; //FIXME: check if theres a better level
                case "schema-qualify": return Level.Error; //FIXME: check if theres a better level
                case "select-star": return Level.Error;
                case "semicolon-termination": return Level.Warning; //FIXME: check if theres a better level
                case "set-ansi": return Level.Error; //FIXME: check if theres a better level
                case "set-nocount": return Level.Error; //FIXME: check if theres a better level
                case "set-quoted-identifier": return Level.Error; //FIXME: check if theres a better level
                case "set-transaction-isolation-level": return Level.Error; //FIXME: check if theres a better level
                case "set-variable": return Level.Error; //FIXME: check if theres a better level
                case "upper-lower": return Level.Error;
                case "unicode-string": return Level.Error;

                default:
                    throw new NotImplementedException($"Should map {rule} rule category");
            }
        }
    }
}