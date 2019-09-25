using OrleansSample.Interfaces;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using System.Linq;

namespace OrleansSample.Client
{
    class Program
    {
        const int initializeAttemptsBeforeFailing = 5;
        private static int attempt = 0;


        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }
        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await StartClientWithRetries())
                {
                    await DoClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                return 1;
            }
        }
        private static async Task<IClusterClient> StartClientWithRetries()
        {
            attempt = 0;
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloWorldApp";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect(RetryFilter);
            Console.WriteLine("Client successfully connect to silo host");
            return client;
        }

        private static async Task<bool> RetryFilter(Exception exception)
        {
            if (exception.GetType() != typeof(SiloUnavailableException))
            {
                Console.WriteLine($"Cluster client failed to connect to cluster with unexpected error.  Exception: {exception}");
                return false;
            }
            attempt++;
            Console.WriteLine($"Cluster client attempt {attempt} of {initializeAttemptsBeforeFailing} failed to connect to cluster.  Exception: {exception}");
            if (attempt > initializeAttemptsBeforeFailing)
            {
                return false;
            }
            await Task.Delay(TimeSpan.FromSeconds(4));
            return true;
        }
        private static async Task DisplayMessages(IMessage msgGrain) 
        {
            Console.WriteLine("=====");
            Console.WriteLine("Retrieving messages...");
            var messages = await msgGrain.GetMessages();
            if(!messages.Any()) {
                Console.WriteLine("No messages found");
            } 
            else 
            {
                Console.WriteLine("---------");
                var p = 0;
                foreach(var m in messages) 
                {
                    var d = $"{p}|{m.PadRight(30)}|";
                    Console.WriteLine(d);
                    p++;
                }
                Console.WriteLine("---------");
            }
            Console.WriteLine("=====");
        }
        private static async Task DoClientWork(IClusterClient client)
        {
            // example of calling grains from the initialized client
            var friend = client.GetGrain<IHelloArchive>(0);
            var response = await friend.SayHello("Good morning, my friend!");
            Console.WriteLine("\n\n{0}\n\n", response);

            var msgGrain = client.GetGrain<IMessage>(0);
            await DisplayMessages(msgGrain);
            Console.WriteLine("Enter input to process... or type 'remove' to remove a message");
            while(true) 
            {
                var readInput = Console.ReadLine();
                if(string.IsNullOrEmpty(readInput))
                    continue;
                if(readInput.Equals("remove"))
                {
                    Console.WriteLine("Enter position");
                    var pos = -1;
                    if(Int32.TryParse(Console.ReadLine(), out pos)) 
                    {
                        await msgGrain.RemoveMessage(pos);
                    }
                    await DisplayMessages(msgGrain);
                }   
                else 
                {
                    var results = await msgGrain.SendMessage(readInput);
                    Console.WriteLine($"Message Response: {results}");
                    await DisplayMessages(msgGrain);
                } 
            }
        }

    }
}
