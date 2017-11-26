# ServiceFabric.ConnectionStrings
It's pretty common for services within in an application to know the address and partitioning scheme of each other. Your service instances might not be static enough that you can hardcode them, but they might not be dynamic enough that it's worth figuring out how to dynamically resolve them. ServiceFabric.ConnectionStrings lets you build connection strings which encapsulate the service's address partitioning information in a single string.
