using System;
using System.Collections.Generic;
using System.Linq;

namespace NickDarvey.ServiceFabric.ConnectionStrings
{
    public class ServiceConnectionString
    {
        private const string ServiceUriKey = "ServiceUri";
        private const string PartitionKindKey = "PartitionKind";
        private const string PartitionCountKey = "PartitionCount";
        private const string PartitionAlgorithmKey = "PartitionAlgorithm";

        private static readonly IEqualityComparer<string> KeyComparer = StringComparer.InvariantCultureIgnoreCase;

        private readonly IReadOnlyDictionary<string, string> ConnectionStringParts;

        public Uri ServiceUri { get => new Uri(ConnectionStringParts[ServiceUriKey]); }
        public string PartitionKind { get => ConnectionStringParts[PartitionKindKey]; }
        public int PartitionCount { get => int.Parse(ConnectionStringParts[PartitionCountKey]); }
        public string PartitionAlgorithm { get => ConnectionStringParts[PartitionAlgorithmKey]; }

        internal ServiceConnectionString(IReadOnlyDictionary<string, string> connectionStringParts)
        {
            if (!connectionStringParts.ContainsKey(ServiceUriKey))
                throw new ArgumentException("A " + ServiceUriKey + " is required.");

            if (!connectionStringParts.ContainsKey(PartitionKindKey))
                throw new ArgumentException("A " + PartitionKindKey + " is required.");

            if (!connectionStringParts.ContainsKey(PartitionCountKey))
                throw new ArgumentException("A " + PartitionCountKey + " is required.");

            if (!Uri.TryCreate(connectionStringParts[ServiceUriKey], UriKind.Absolute, out var _))
                throw new ArgumentException($"'{connectionStringParts[ServiceUriKey]}' is not a valid absolute URI.");

            if (!int.TryParse(connectionStringParts[PartitionCountKey], out var _))
                throw new ArgumentException($"'{connectionStringParts[PartitionCountKey]}' is not a valid integer.");

            ConnectionStringParts = connectionStringParts;
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
            catch(ArgumentException)
            {
                result = default(ServiceConnectionString);
                return false;
            }
        }

        public static explicit operator ServiceConnectionString(string s) =>
            Parse(s);
    }
}
