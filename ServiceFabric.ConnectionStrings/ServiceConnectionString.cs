using System;
using System.Collections.Generic;
using System.Linq;

namespace NickDarvey.ServiceFabric.ConnectionStrings
{
    public class ServiceConnectionString
    {
        private const string ServiceUriKey = "ServiceUri";
        private const string PartitionKindKey = "PartitionKind";
        private const string PartitionKindInt64RangeValue = "Int64Range";
        private const string PartitionKindNamedValue = "Named";
        private const string PartitionKindSingletonValue = "Singleton";
        private const string PartitionCountKey = "PartitionCount";
        private const string PartitionAlgorithmKey = "PartitionAlgorithm";

        private static readonly IEqualityComparer<string> KeyComparer = StringComparer.InvariantCultureIgnoreCase;

        public Uri ServiceUri { get; }
        public string PartitionKind { get; }
        public int PartitionCount { get; }
        public string PartitionAlgorithm { get; }

        internal ServiceConnectionString(IReadOnlyDictionary<string, string> connectionStringParts)
        {
            ServiceUri =
                connectionStringParts.ContainsKey(ServiceUriKey) == false ? throw new ArgumentException($"A {ServiceUriKey} is required.")
                : Uri.TryCreate(connectionStringParts[ServiceUriKey], UriKind.RelativeOrAbsolute, out var serviceUri) == false ? throw new ArgumentException($"'{connectionStringParts[ServiceUriKey]}' is not a valid URI.")
                : serviceUri;

            PartitionKind =
                connectionStringParts.ContainsKey(PartitionKindKey) == false ? throw new ArgumentException($"A {PartitionKindKey} is required.")
                : connectionStringParts[PartitionKindKey] != PartitionKindInt64RangeValue &&
                  connectionStringParts[PartitionKindKey] != PartitionKindNamedValue &&
                  connectionStringParts[PartitionKindKey] != PartitionKindSingletonValue ? throw new ArgumentException($"A {PartitionKindKey} must be '{PartitionKindInt64RangeValue}', '{PartitionKindNamedValue}' or '{PartitionKindSingletonValue}'.")
                : connectionStringParts[PartitionKindKey];

            PartitionCount =
                connectionStringParts.ContainsKey(PartitionCountKey) == false &&
                connectionStringParts[PartitionKindKey] == PartitionKindInt64RangeValue ? throw new ArgumentException($"A {PartitionCountKey} is required when the {PartitionKindKey} is '{connectionStringParts[PartitionKindKey]}'.")
                : connectionStringParts.ContainsKey(PartitionCountKey) == false ? -1
                : int.TryParse(connectionStringParts[PartitionCountKey], out var partitionCount) == false ? throw new ArgumentException($"'{connectionStringParts[PartitionCountKey]}' is not a valid integer.")
                : partitionCount;

            PartitionAlgorithm =
                connectionStringParts.TryGetValue(PartitionAlgorithmKey, out var partitionAlgorithm) == false ? default(string)
                : partitionAlgorithm;
        }

        public static ServiceConnectionString Parse(string s) =>
            new ServiceConnectionString(
            s.Split(';')
            .Select(t => t.Split(new char[] { '=' }, 2))
            .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), KeyComparer));

        public static bool TryParse(string s, out ServiceConnectionString result)
        {
            try
            {
                result = Parse(s);
                return true;
            }
            catch (ArgumentException)
            {
                result = default(ServiceConnectionString);
                return false;
            }
        }

        public static explicit operator ServiceConnectionString(string s) =>
            Parse(s);
    }
}
