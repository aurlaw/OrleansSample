# Orleans Sample

#### Building the sample
```
dotnet restore
```

This sample uses SQL Server Storage via ADO.NET Grain Storage Provider.

You must create a SQL database then execute the Main and Persistance scripts under SQLServer.

Update the ```appsettings.json``` within the SiloHost project.

Also, this sample supports Azure Table Storage instead of SQL Server. Just set  ```"StorageType": "AzureTable"```, ```"AzureTableName": "YOUR AZURE TABLE NAME"``` and the ```"OrleansConnectionString"``` to your Azure Table connection string.

#### Running the sample
From Visual Studio, you can start start the SiloHost and OrleansClient projects simultaneously (you can set up multiple startup projects by right-clicking the solution in the Solution Explorer, and select `Set StartUp projects`.

Alternatively, you can run from the command line:

To start the silo
```
dotnet run --project OrleansSample.SiloHost
```
Silo is configured with Orleans dashboard on port 9191.
http://localhost:9191/


To start the client (you will have to use a different command window)

Console Client
```
dotnet run --project OrleansSample.Client
```

or 

Web Client
```
dotnet run --project OrleansSample.Web
```

https://localhost:5001/message/list
https://localhost:5001/message/add/{message}
https://localhost:5001/message/remove/{position}



#### TODO

- Web app
- SignalR
- Streams
- Modify conf settings for xml support files: 
 https://github.com/dotnet/orleans/tree/master/src/SDK
 - https://medium.com/@kritner/microsoft-orleans-easily-switching-between-development-and-production-configurations-20e109be6458