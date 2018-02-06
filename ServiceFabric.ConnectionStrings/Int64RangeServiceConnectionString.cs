using System;
using System.Collections.Generic;

namespace NickDarvey.ServiceFabric.ConnectionStrings
{
    public class Int64RangeServiceConnectionString : ServiceConnectionString
    {
        private const string PartitionKindInt64RangeValue = "Int64Range";
        private const string PartitionLowKeyKey = "PartitionLowKey";
        private const string PartitionHighKeyKey = "PartitionHighKey";
        private const string PartitionAlgorithmKey = "PartitionAlgorithm";

        public long PartitionLowKey { get; }
        public long PartitionHighKey { get; }
        public string PartitionAlgorithm { get; }

        public Int64RangeServiceConnectionString(
            Uri serviceUri,
            long partitionLowKey,
            long partitionHighKey,,
            string partitionAlgorithm) : this(
            partitionLowKey: partitionLowKey,
            partitionHighKey: partitionHighKey,
            partitionAlgorithm: partitionAlgorithm,
            parts: new ServiceConnectionString(serviceUri, PartitionKindInt64RangeValue))
        { }

        internal Int64RangeServiceConnectionString(
            long partitionLowKey,
            long partitionHighKey,
            string partitionAlgorithm,
            ServiceConnectionString parts)
            : base(parts)
        {
            PartitionLowKey = partitionLowKey;

            PartitionHighKey = partitionHighKey;

            PartitionAlgorithm = !string.IsNullOrWhiteSpace(partitionAlgorithm)
                ? partitionAlgorithm : throw new ArgumentException("A partition algorithm must be defined");
        }

        public static new Int64RangeServiceConnectionString Parse(string connectionString)
        {
            var parts = ServiceConnectionString.Parse(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
            return Parse(parts);
        }

        public static new Int64RangeServiceConnectionString Parse(IDictionary<string, string> parts)
        {
            var serviceConnectionString = ServiceConnectionString.Parse(parts ?? throw new ArgumentNullException(nameof(parts)));
            return Parse(serviceConnectionString);
        }

        private static Int64RangeServiceConnectionString Parse(ServiceConnectionString parts)
        {
            if (parts.PartitionKind != PartitionKindInt64RangeValue)
                throw new ArgumentException($"Connection string is not of type '{PartitionKindInt64RangeValue}'.");

            var partitionLowKey =
                 parts.ContainsKey(PartitionLowKeyKey) == false ? throw new ArgumentException($"A {PartitionLowKeyKey} is required when the {nameof(PartitionKind)} is '{PartitionKindInt64RangeValue}'.")
                 : long.TryParse(parts[PartitionLowKeyKey], out var lowKey) == false ? throw new ArgumentException($"'{parts[PartitionLowKeyKey]}' is not a valid long.")
                 : lowKey;

            var partitionHighKey =
                parts.ContainsKey(PartitionHighKeyKey) == false ? throw new ArgumentException($"A {PartitionHighKeyKey} is required when the {nameof(PartitionKind)} is '{PartitionKindInt64RangeValue}'.")
                : long.TryParse(parts[PartitionHighKeyKey], out var highKey) == false ? throw new ArgumentException($"'{parts[PartitionHighKeyKey]}' is not a valid long.")
                : highKey;

            var partitionAlgorithm =
                parts.TryGetValue(PartitionAlgorithmKey, out var algorithm) == false ? default
                : algorithm;

            return new Int64RangeServiceConnectionString(partitionLowKey, partitionHighKey, partitionAlgorithm, parts);
        }

        public static explicit operator Int64RangeServiceConnectionString(string s) =>
            Parse(s);
    }
}
