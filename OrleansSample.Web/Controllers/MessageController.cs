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
    [Route("[controller]/[action]")]
    public class MessageController : Controller
    {
        private readonly IClusterClient orleansClient;

        public MessageController(IClusterClient orleansClient)
        {
            this.orleansClient = orleansClient;

        }
        //message/list
        public async Task<IActionResult> List()
        {
            var msgGrain = orleansClient.GetGrain<IMessage>(0);
            var messages = await msgGrain.GetMessages();

            return Ok(messages);
        }
        //message/add/{name}
        [HttpGet("{name}")]       
        public async Task<IActionResult> Add(string name) 
        {
            var msgGrain = orleansClient.GetGrain<IMessage>(0);
            var result = await msgGrain.SendMessage(name);
            return Ok($"Added {result}");
        }
        //message/remove/{id}
        [HttpGet("{id}")]       
        public async Task<IActionResult> Remove(int id) 
        {
            var msgGrain = orleansClient.GetGrain<IMessage>(0);
            await msgGrain.RemoveMessage(id);
            return Ok($"Remove {id}");
        }
    }
}