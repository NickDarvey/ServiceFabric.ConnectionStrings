using System;
using System.Collections.Generic;

namespace NickDarvey.ServiceFabric.ConnectionStrings
{
    public class SingletonServiceConnectionString : ServiceConnectionString
    {
        private const string PartitionKindSingleton = "Singleton";

        public SingletonServiceConnectionString(
            Uri serviceUri) : this(
            parts: new ServiceConnectionString(serviceUri, PartitionKindSingleton))
        { }

        internal SingletonServiceConnectionString(
            ServiceConnectionString parts)
            : base(parts)
        { }

        public static new SingletonServiceConnectionString Parse(string connectionString)
        {
            var parts = ServiceConnectionString.Parse(connectionString ?? throw new ArgumentNullException(nameof(connectionString)));
            return new SingletonServiceConnectionString(parts);
        }

        public static new SingletonServiceConnectionString Parse(IDictionary<string, string> parts)
        {
            var serviceConnectionString = ServiceConnectionString.Parse(parts ?? throw new ArgumentNullException(nameof(parts)));
            return new SingletonServiceConnectionString(serviceConnectionString);
        }

        public static explicit operator SingletonServiceConnectionString(string s) =>
            Parse(s);
    }
}
