using Orleans;
using Orleans.Streams;
using OrleansSample.Interfaces;
using OrleansSample.Interfaces.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace OrleansSample.Web.Services
{
    public interface ISubscriptionManager
    {
         Task<Guid> CreateSubscription(HubCallerContext context, Guid subscriptionId, Action<SubscriptionEventArgs> action);
    }
}