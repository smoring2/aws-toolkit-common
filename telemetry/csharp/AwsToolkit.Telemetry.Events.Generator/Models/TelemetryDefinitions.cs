// ReSharper disable InconsistentNaming
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Amazon.AwsToolkit.Telemetry.Events.Generator.Models
{
    public class TelemetryDefinitions
    {
        /// <summary>
        /// Pattern that metric names must match: lowercase start, alphanumeric segments separated by underscores.
        /// This prevents code injection through metric names used in generated source text.
        /// </summary>
        private static readonly Regex MetricNamePattern = new Regex(@"^[a-z][a-zA-Z0-9]*(_[a-zA-Z0-9]+)+$");

        /// <summary>
        /// Pattern that type names must match: alphanumeric identifier starting with a letter.
        /// </summary>
        private static readonly Regex TypeNamePattern = new Regex(@"^[a-zA-Z][a-zA-Z0-9]*(_[a-zA-Z0-9]+)*$");

        /// <summary>
        /// Pattern that allowed values must match: safe characters only.
        /// </summary>
        private static readonly Regex AllowedValuePattern = new Regex(@"^[a-zA-Z0-9 ,._-]*$");

        public MetricType[] types { get; set; }
        public Metric[] metrics { get; set; }


        public static TelemetryDefinitions Load(string filename)
        {
            var json = File.ReadAllText(filename);

            var doc = JsonConvert.DeserializeObject<TelemetryDefinitions>(json);

            // Validate all inputs to prevent code injection through definition strings.
            // This is especially important on the -s/--SupplementalDefinitions path which
            // accepts externally-supplied definitions files with no prior schema validation.
            ValidateDefinitions(doc, filename);

            // Process all metric metadata so that null "required" fields are set to true
            doc.metrics.ToList().ForEach(m => m.metadata?.ToList().ForEach(data =>
            {
                if (!data.required.HasValue)
                {
                    data.required = true;
                }
            }));

            return doc;
        }

        /// <summary>
        /// Validates that all names and values in the definitions conform to safe patterns,
        /// preventing code injection into generated source text.
        /// </summary>
        private static void ValidateDefinitions(TelemetryDefinitions definitions, string filename)
        {
            if (definitions.metrics == null)
            {
                throw new ArgumentException($"Definitions file '{filename}' is missing required 'metrics' field.");
            }

            if (definitions.types != null)
            {
                foreach (var type in definitions.types)
                {
                    if (string.IsNullOrEmpty(type.name) || !TypeNamePattern.IsMatch(type.name))
                    {
                        throw new ArgumentException(
                            $"Invalid type name '{type.name}' in definitions file '{filename}'. " +
                            $"Type names must match pattern: {TypeNamePattern}");
                    }

                    if (type.allowedValues != null)
                    {
                        foreach (var value in type.allowedValues)
                        {
                            if (!AllowedValuePattern.IsMatch(value))
                            {
                                throw new ArgumentException(
                                    $"Invalid allowed value '{value}' for type '{type.name}' in definitions file '{filename}'. " +
                                    $"Allowed values must match pattern: {AllowedValuePattern}");
                            }
                        }
                    }
                }
            }

            foreach (var metric in definitions.metrics)
            {
                if (string.IsNullOrEmpty(metric.name) || !MetricNamePattern.IsMatch(metric.name))
                {
                    throw new ArgumentException(
                        $"Invalid metric name '{metric.name}' in definitions file '{filename}'. " +
                        $"Metric names must match pattern: {MetricNamePattern}");
                }
            }
        }
    }
}
