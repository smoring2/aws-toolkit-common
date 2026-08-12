using System.Linq;
using System.Text.RegularExpressions;

namespace Amazon.AwsToolkit.Telemetry.Events.Generator.Utils
{
    public static class StringExtensionMethods
    {
        // Matches any character that is NOT a letter, digit, or underscore
        private static readonly Regex InvalidIdentifierCharsRegex = new Regex(@"[^\p{L}\p{Nd}_]", RegexOptions.Compiled);

        // Matches a leading digit (C# identifiers cannot start with a digit)
        private static readonly Regex LeadingDigitRegex = new Regex(@"^\d", RegexOptions.Compiled);

        public static string ToPascalCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (text.Length == 1)
            {
                return text.ToUpper();
            }

            var segments = text.Split("_");
            if (segments.Length > 1)
            {
                return string.Concat(segments.Select(ToPascalCase));
            }

            return $"{text[0].ToString().ToUpper()}{text.Substring(1)}";
        }

        /// <summary>
        /// Sanitizes a string to produce a valid C# identifier by removing all characters
        /// that are not Unicode letters, digits, or underscores. If the result starts with
        /// a digit, an underscore is prepended. Returns an empty string if input is null or empty.
        /// </summary>
        public static string ToSafeIdentifier(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // Remove all characters that are not valid in a C# identifier
            var sanitized = InvalidIdentifierCharsRegex.Replace(text, string.Empty);

            // Ensure the identifier does not start with a digit
            if (!string.IsNullOrEmpty(sanitized) && LeadingDigitRegex.IsMatch(sanitized))
            {
                sanitized = "_" + sanitized;
            }

            return sanitized;
        }
    }
}
