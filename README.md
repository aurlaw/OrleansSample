# Orleans Sample

#### Building the sample
```
dotnet restore
```

This sample uses SQL Server Storage via ADO.NET Grain Storage Provider.

You must create a SQL database then execute the Main and Persistance scripts under SQLServer.

Update the ```appsettings.json``` within the SiloHost project.


#### Running the sample
From Visual Studio, you can start start the SiloHost and OrleansClient projects simultaneously (you can set up multiple startup projects by right-clicking the solution in the Solution Explorer, and select `Set StartUp projects`.

Alternatively, you can run from the command line:

To start the silo
```
dotnet run --project OrleansSample.SiloHost
```


To start the client (you will have to use a different command window)
```
dotnet run --project OrleansSample.Client
```

#### TODO

read from console
