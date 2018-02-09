# ServiceFabric.ConnectionStrings
It's pretty common for services within in an application to know the address and partitioning scheme of each other. Your service instances might not be static enough that you can hardcode them, but they might not be dynamic enough that it's worth figuring out how to dynamically resolve them. ServiceFabric.ConnectionStrings lets you build connection strings which encapsulate the service's address partitioning information in a single string.

You can grab it as a PowerShell module from [PowerShell Gallery](https://www.powershellgallery.com/packages/NickDarvey.ServiceFabric.ConnectionStrings) or as a NuGet package from [nuget.org](https://www.nuget.org/packages/NickDarvey.ServiceFabric.ConnectionStrings).

## NuGet
### Getting started
```
Install-Package NickDarvey.ServiceFabric.ConnectionStrings
```

### Usage
```csharp
var connectionStringString = "PartitionLowKey=0;PartitionHighKey=31;PartitionKind=Int64Range;PartitionAlgorithm=Fnv;ServiceUri=fabric:/MyApp/MyService";

var connectionStringFromParams = new Int64RangeServiceConnectionString(
    serviceUri: new Uri("fabric:/MyApp/MyService"),
    partitionLowKey: 0,
    partitionHighKey: 31,
    partitionAlgorithm: "MyApp:Fnv");

var connectionStringFromParsing = Int64RangeServiceConnectionString.Parse(
    connectionStringString);

Assert.Equal(connectionStringString, connectionStringFromParams.ToString());
Assert.Equal(connectionStringFromParams.ServiceUri, connectionStringFromParsing.ServiceUri);
Assert.Equal(connectionStringFromParams.PartitionLowKey, connectionStringFromParsing.PartitionLowKey);
Assert.Equal(connectionStringFromParams.PartitionHighKey, connectionStringFromParsing.PartitionHighKey);
Assert.Equal(connectionStringFromParams.PartitionAlgorithm, connectionStringFromParsing.PartitionAlgorithm);
```

## PowerShell
### Getting started
```powershell
Get-PackageProvider -Name NuGet -ForceBootstrap | Out-Null
Install-Module -Name NickDarvey.ServiceFabric.ConnectionStrings -Scope CurrentUser -Force -AllowClobber
```

### Usage
```powershell
$serviceConfiguration = @{
    ServiceUri = "fabric:/MyApp/MyService";
    PartitionKind = "Int64Range";
    PartitionAlgorithm = "Fnv";
    LowKey = 0;
    HighKey = 31;
}

$connectionString = ConvertTo-ServiceConnectionString $serviceConfiguration
"PartitionAlgorithm=Fnv;HighKey=31;LowKey=0;ServiceUri=fabric:/MyApp/MyService;PartitionKind=Int64Range" -eq $connectionString.ToString()

>True
```