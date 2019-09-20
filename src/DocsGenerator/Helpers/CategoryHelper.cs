using System;
using Codacy.Engine.Seed.Patterns;

namespace Codacy.TSQLLint.DocsGenerator.Helpers
{
    /// <summary>
    /// Helper to convert to codacy category from a TSQLLint rule
    /// </summary>
    public static class CategoryHelper
    {
        /// <summary>
        /// This map a rule to a codacy category.
        /// </summary>
        /// <param name="rule">rule name</param>
        /// <returns>Codacy category</returns>
        /// <exception cref="NotImplementedException">When found a non explicit mapped rule</exception>
        public static Category ToCategory(string rule)
        {
            switch (rule)
            {
                case "conditional-begin-end": return Category.ErrorProne;
                case "cross-database-transaction": return Category.ErrorProne;
                case "data-compression": return Category.Performance;
                case "data-type-length": return Category.Performance;
                case "disallow-cursors": return Category.Performance;
                case "full-text": return Category.Performance;
                case "information-schema": return Category.ErrorProne;
                case "keyword-capitalization": return Category.CodeStyle;
                case "linked-server": return Category.Performance;
                case "multi-table-alias": return Category.ErrorProne;
                case "named-constraint": return Category.ErrorProne;
                case "non-sargable": return Category.Performance;
                case "object-property": return Category.ErrorProne;
                case "print-statement": return Category.ErrorProne; //FIXME: check if theres a better category
                case "schema-qualify": return Category.ErrorProne; //FIXME: check if theres a better category
                case "select-star": return Category.ErrorProne;
                case "semicolon-termination": return Category.CodeStyle;
                case "set-ansi": return Category.ErrorProne; //FIXME: check if theres a better category
                case "set-nocount": return Category.ErrorProne; //FIXME: check if theres a better category
                case "set-quoted-identifier": return Category.ErrorProne; //FIXME: check if theres a better category
                case "set-transaction-isolation-level": return Category.ErrorProne; //FIXME: check if theres a better category
                case "set-variable": return Category.ErrorProne; //FIXME: check if theres a better category
                case "upper-lower": return Category.ErrorProne;
                case "unicode-string": return Category.ErrorProne;

                default:
                    throw new NotImplementedException($"Should map {rule} rule category");
            }
        }
    }
}