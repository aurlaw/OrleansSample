using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using OrleansSample.Web.Models;
using OrleansSample.Interfaces;

namespace OrleansSample.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/messages")]
    public class MessageController : Controller
    {
        private readonly IClusterClient orleansClient;

        public MessageController(IClusterClient orleansClient)
        {
            this.orleansClient = orleansClient;

        }
        //api/messages/list
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var msgGrain = orleansClient.GetGrain<IMessage>(0);
            var messages = await msgGrain.GetMessages();
            var results = messages.Select((message, position) => new {position, message});
            return Ok(results);
        }
        //api/messages/{name}
        [HttpPut("{name}")]       
        public async Task<IActionResult> Put([FromRoute]string name) 
        {
            var msgGrain = orleansClient.GetGrain<IMessage>(0);
            var result = await msgGrain.SendMessage(name);
            return Ok($"Added {result}");
        }
        //api/messages/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            var msgGrain = orleansClient.GetGrain<IMessage>(0);
            var message = await msgGrain.GetMessage(id);
            return Ok(message);
        }
        //api/messages/{id}
        [HttpDelete("{id}")]       
        public async Task<IActionResult> Delete([FromRoute]int id) 
        {
            var msgGrain = orleansClient.GetGrain<IMessage>(0);
            await msgGrain.RemoveMessage(id);
            return Ok($"Remove {id}");
        }
    }
}