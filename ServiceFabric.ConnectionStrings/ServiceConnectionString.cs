using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NickDarvey.ServiceFabric.ConnectionStrings
{
    public class ServiceConnectionString : ReadOnlyDictionary<string, string>, IEquatable<ServiceConnectionString>
    {
        private const string ServiceUriKey = "ServiceUri";
        private const string PartitionKindKey = "PartitionKind";

        private const string PartitionKindNamedValue = "Named";
        private const string PartitionKindSingletonValue = "Singleton";

        private static readonly IEqualityComparer<string> KeyComparer = StringComparer.OrdinalIgnoreCase;

        public Uri ServiceUri { get; }
        public string PartitionKind { get; }

        public ServiceConnectionString(
            Uri serviceUri,
            string partitionKind) : this(
            serviceUri: serviceUri,
            partitionKind: partitionKind,
            parts: new Dictionary<string, string> {
            { ServiceUriKey, serviceUri.ToString() },
            { PartitionKindKey, partitionKind },})
        { }

        protected ServiceConnectionString(
            ServiceConnectionString parts) : this(
            serviceUri: parts.ServiceUri,
            partitionKind: parts.PartitionKind,
            parts: parts)
        { }

        private ServiceConnectionString(
            Uri serviceUri,
            string partitionKind,
            IDictionary<string, string> parts)
            : base(parts)
        {
            ServiceUri = serviceUri ?? throw new ArgumentNullException(nameof(serviceUri));
            PartitionKind = partitionKind ?? throw new ArgumentNullException(nameof(partitionKind));
        }

        public static ServiceConnectionString Parse(string connectionString)
        {
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            var parts = (connectionString.Trim().EndsWith(";")
                ? connectionString.Trim().TrimEnd(';')
                : connectionString)
                .Split(';')
                .Select(t => t.Split(new char[] { '=' }, 2))
                .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), KeyComparer);

            return Parse(parts);
        }

        public static ServiceConnectionString Parse(IDictionary<string, string> parts)
        {
            if (parts == default(IDictionary<string, string>))
                throw new ArgumentNullException(nameof(parts));

            var serviceUri =
                parts.ContainsKey(ServiceUriKey) == false ? throw new ArgumentException($"A {ServiceUriKey} is required.")
                : Uri.TryCreate(parts[ServiceUriKey], UriKind.RelativeOrAbsolute, out var uri) == false ? throw new ArgumentException($"'{parts[ServiceUriKey]}' is not a valid URI.")
                : uri;

            var partitionKind =
                parts.ContainsKey(PartitionKindKey) == false ||
                string.IsNullOrWhiteSpace(parts[PartitionKindKey]) ? throw new ArgumentException($"A {PartitionKindKey} is required.")
                : parts[PartitionKindKey];

            return new ServiceConnectionString(serviceUri, partitionKind, parts);
        }

        private static IDictionary<string, string> ParseConnectionString(string connectionString) =>
            (connectionString.Trim().EndsWith(";")
            ? connectionString.Trim().TrimEnd(';')
            : connectionString)
            .Split(';')
            .Select(t => t.Split(new char[] { '=' }, 2))
            .ToDictionary(t => t[0].Trim(), t => t[1].Trim(), KeyComparer);

        private static string CreateConnectionString(ServiceConnectionString connectionString) =>
            string.Join(";", connectionString.Select(kv => kv.Key + "=" + kv.Value));

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
            ServiceUri.Equals(other?.ServiceUri) &&
            PartitionKind.Equals(other?.PartitionKind);

        public override bool Equals(object other) =>
            other is ServiceConnectionString o
            ? Equals(o) : false;

        public static bool operator ==(ServiceConnectionString lhs, ServiceConnectionString rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(ServiceConnectionString lhs, ServiceConnectionString rhs) =>
            !lhs.Equals(rhs);

        public override int GetHashCode()
        {
            var hashCode = -730815131;
            hashCode = hashCode * -1521134295 + EqualityComparer<Uri>.Default.GetHashCode(ServiceUri);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PartitionKind);
            return hashCode;
        }

        public override string ToString() =>
            CreateConnectionString(this);
    }
}
