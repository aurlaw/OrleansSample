using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using OrleansSample.Interfaces;

namespace OrleansSample.Web.Hubs
{
    public class TodoHub : Hub
    {
        private readonly IClusterClient orleansClient;
        private readonly ITodo todoGrain;
        public TodoHub(IClusterClient orleansClient)
        {
            this.orleansClient = orleansClient;
            this.todoGrain = orleansClient.GetGrain<ITodo>(Constants.TodoKey);

        }
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task Subscribe()
        {
            var result = await todoGrain.GetAllAsync();

            await Clients.All.SendAsync("SubscribeReceived", result);
        }
    }
}
