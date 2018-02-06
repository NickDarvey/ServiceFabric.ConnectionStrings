# ServiceFabric.ConnectionStrings
It's pretty common for services within in an application to know the address and partitioning scheme of each other. Your service instances might not be static enough that you can hardcode them, but they might not be dynamic enough that it's worth figuring out how to dynamically resolve them. ServiceFabric.ConnectionStrings lets you build connection strings which encapsulate the service's address partitioning information in a single string.

You can grab it as a PowerShell module from [PowerShell Gallery](https://www.powershellgallery.com/packages/NickDarvey.ServiceFabric.ConnectionStrings) or as a NuGet package from [nuget.org](https://www.nuget.org/packages/NickDarvey.ServiceFabric.ConnectionStrings).

## Using the NuGet package
```c#
var connectionString = new Int64RangeServiceConnectionString(
    serviceUri: new Uri("fabric:/MyApp/MyService"),
    partitionLowKey: 0,
    partitionHighKey: 31,
    partitionAlgorithm: "MyApp:Fnv")

Assert.Equal(
    "PartitionLowKey=0;PartitionHighKey=31;PartitionKind=Int64Range;PartitionAlgorithm=Fnv;ServiceUri=fabric:/MyApp/MyService",
    connectionString.ToString()
);
```
