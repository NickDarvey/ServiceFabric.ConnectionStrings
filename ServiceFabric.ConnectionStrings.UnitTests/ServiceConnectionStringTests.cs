using NickDarvey.ServiceFabric.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NickDarvey.ServiceFabric.ConnectionStrings
{
    public class ServiceConnectionStringTests
    {
        [Theory]
        [InlineData(
            "PartitionKind=Int64Range;PartitionAlgorithm=Fnv;PartitionCount=32;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService",
            "Int64Range", "http://dev.local:19081/Reactor/default/ExtractorService")]
        [InlineData(
            "PartitionAlgorithm=Fnv;PartitionCount=32;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService;PartitionKind=Int64Range;",
            "Int64Range", "http://dev.local:19081/Reactor/default/ExtractorService")]
        public void Parse_Valid_Parses(
            string connectionString,
            string partitionKind,
            string serviceUri)
        {
            var result = ServiceConnectionString.Parse(connectionString);

            Assert.Equal(partitionKind, result.PartitionKind);
            Assert.Equal(serviceUri, result.ServiceUri.ToString());
        }

        [Theory]
        [InlineData(
            "PartitionKind=Int64Range;PartitionAlgorithm=Fnv;PartitionCount=32")]
        public void Parse_InvalidServiceUri_Throws(string connectionString)
        {
            Assert.Throws<ArgumentException>(() => ServiceConnectionString.Parse(connectionString));
        }

        [Theory]
        [InlineData(
            "PartitionKind=;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService")]
        [InlineData(
            "ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService")]
        public void Parse_InvalidPartitionKind_Throws(string connectionString)
        {
            Assert.Throws<ArgumentException>(() => ServiceConnectionString.Parse(connectionString));
        }
    }
}
