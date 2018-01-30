using NickDarvey.ServiceFabric.ConnectionStrings;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NickDarvey.ServiceFabric.ConnectionStrings
{
    public class Int64RangeServiceConnectionStringTests
    {
        [Theory]
        [InlineData(
            "PartitionLowKey=0;PartitionHighKey=0;PartitionKind=Int64Range;PartitionAlgorithm=Fnv;PartitionCount=1;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService",
            "Int64Range", 0, 0, "Fnv", 1, "http://dev.local:19081/Reactor/default/ExtractorService")]
        [InlineData(
            "PartitionLowKey=0;PartitionHighKey=1;PartitionKind=Int64Range;PartitionAlgorithm=Fnv;PartitionCount=2;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService",
            "Int64Range", 0, 1, "Fnv", 2, "http://dev.local:19081/Reactor/default/ExtractorService")]
        [InlineData(
            "PartitionLowKey=0;PartitionHighKey=31;PartitionKind=Int64Range;PartitionAlgorithm=Fnv;PartitionCount=32;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService",
            "Int64Range", 0, 31, "Fnv", 32, "http://dev.local:19081/Reactor/default/ExtractorService")]
        [InlineData(
            "PartitionAlgorithm=Fnv;PartitionCount=32;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService;PartitionKind=Int64Range;PartitionLowKey=0;PartitionHighKey=31;",
            "Int64Range", 0, 31, "Fnv", 32, "http://dev.local:19081/Reactor/default/ExtractorService")]
        public void Parse_Valid_Parses(
            string connectionString,
            string partitionKind,
            long partitionLowKey,
            long partitionHighKey,
            string partitionAlgorithm,
            int partitionCount,
            string serviceUri)
        {
            var result = Int64RangeServiceConnectionString.Parse(connectionString);

            Assert.Equal(partitionKind, result.PartitionKind);
            Assert.Equal(partitionHighKey, result.PartitionHighKey);
            Assert.Equal(partitionLowKey, result.PartitionLowKey);
            Assert.Equal(partitionAlgorithm, result.PartitionAlgorithm);
            Assert.Equal(partitionCount, result.PartitionCount);
            Assert.Equal(serviceUri, result.ServiceUri.ToString());
        }

        [Theory]
        [InlineData(
            "PartitionLowKey=cheese;PartitionHighKey=32;PartitionKind=Int64Range;PartitionAlgorithm=Fnv;PartitionCount=32;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService")]
        [InlineData(
            "PartitionHighKey=32;PartitionKind=Int64Range;PartitionAlgorithm=Fnv;PartitionCount=32;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService")]
        public void Parse_InvalidPartitionLowKey_Throws(string connectionString)
        {
            Assert.Throws<ArgumentException>(() => Int64RangeServiceConnectionString.Parse(connectionString));
        }

        [Theory]
        [InlineData(
            "PartitionLowKey=0;PartitionHighKey=cheese;PartitionHighKey=32;PartitionKind=Int64Range;PartitionAlgorithm=Fnv;PartitionCount=32;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService")]
        [InlineData(
            "PartitionLowKey=0;PartitionKind=Int64Range;PartitionAlgorithm=Fnv;PartitionCount=32;ServiceUri=http://dev.local:19081/Reactor/default/ExtractorService")]
        public void Parse_InvalidPartitionHighKey_Throws(string connectionString)
        {
            Assert.Throws<ArgumentException>(() => Int64RangeServiceConnectionString.Parse(connectionString));
        }
    }
}
