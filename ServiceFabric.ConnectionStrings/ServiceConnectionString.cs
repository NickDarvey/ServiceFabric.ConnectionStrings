using System;
using System.Collections.Generic;
using System.Linq;

namespace NickDarvey.ServiceFabric.ConnectionStrings
{
    public class ServiceConnectionString : IEquatable<ServiceConnectionString>
    {
        private const string ServiceUriKey = "ServiceUri";
        private const string PartitionKindKey = "PartitionKind";
        private const string PartitionKindInt64RangeValue = "Int64Range";
        private const string PartitionKindNamedValue = "Named";
        private const string PartitionKindSingletonValue = "Singleton";
        private const string PartitionCountKey = "PartitionCount";
        private const string PartitionAlgorithmKey = "PartitionAlgorithm";

        private static readonly IEqualityComparer<string> KeyComparer = StringComparer.InvariantCultureIgnoreCase;

        private readonly string ConnectionString;

        public Uri ServiceUri { get; }
        public string PartitionKind { get; }
        public int PartitionCount { get; }
        public string PartitionAlgorithm { get; }

        internal ServiceConnectionString(string connectionString)
        {
            ConnectionString = connectionString;

            var parts = connectionString.Split(';')
                .Select(t => t.Split(new char[] { '=' }, 2))
                .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), KeyComparer);

            ServiceUri =
                parts.ContainsKey(ServiceUriKey) == false ? throw new ArgumentException($"A {ServiceUriKey} is required.")
                : Uri.TryCreate(parts[ServiceUriKey], UriKind.RelativeOrAbsolute, out var serviceUri) == false ? throw new ArgumentException($"'{parts[ServiceUriKey]}' is not a valid URI.")
                : serviceUri;

            PartitionKind =
                parts.ContainsKey(PartitionKindKey) == false ? throw new ArgumentException($"A {PartitionKindKey} is required.")
                : parts[PartitionKindKey] != PartitionKindInt64RangeValue &&
                  parts[PartitionKindKey] != PartitionKindNamedValue &&
                  parts[PartitionKindKey] != PartitionKindSingletonValue ? throw new ArgumentException($"A {PartitionKindKey} must be '{PartitionKindInt64RangeValue}', '{PartitionKindNamedValue}' or '{PartitionKindSingletonValue}'.")
                : parts[PartitionKindKey];

            PartitionCount =
                parts.ContainsKey(PartitionCountKey) == false &&
                parts[PartitionKindKey] == PartitionKindInt64RangeValue ? throw new ArgumentException($"A {PartitionCountKey} is required when the {PartitionKindKey} is '{parts[PartitionKindKey]}'.")
                : parts.ContainsKey(PartitionCountKey) == false ? -1
                : int.TryParse(parts[PartitionCountKey], out var partitionCount) == false ? throw new ArgumentException($"'{parts[PartitionCountKey]}' is not a valid integer.")
                : partitionCount;

            PartitionAlgorithm =
                parts.TryGetValue(PartitionAlgorithmKey, out var partitionAlgorithm) == false ? default(string)
                : partitionAlgorithm;
        }

        public static ServiceConnectionString Parse(string s) =>
            new ServiceConnectionString(s);

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

        public bool Equals(ServiceConnectionString other) =>
            ServiceUri == other?.ServiceUri &&
            PartitionKind == other?.PartitionKind &&
            PartitionCount == other?.PartitionCount &&
            PartitionAlgorithm == other?.PartitionAlgorithm;

        public override bool Equals(object other) =>
            other is ServiceConnectionString o
            ? Equals(o) : false;

        public static bool operator ==(ServiceConnectionString lhs, ServiceConnectionString rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(ServiceConnectionString lhs, ServiceConnectionString rhs) =>
            !lhs.Equals(rhs);

        public override int GetHashCode() =>
            new
            {
                ServiceUri = ServiceUri,
                PartitionKind = PartitionKind,
                PartitionCount = PartitionCount,
                PartitionAlgorithm = PartitionAlgorithm
            }.GetHashCode();

        public override string ToString() => ConnectionString;
    }
}
