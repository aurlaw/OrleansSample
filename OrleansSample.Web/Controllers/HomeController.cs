using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using OrleansSample.Web.Models;
using OrleansSample.Interfaces;
using Microsoft.AspNetCore.Http;
using OrleansSample.Interfaces.Models;
using OrleansSample.Web.Services;

namespace OrleansSample.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClusterClient orleansClient;
        private readonly IImageService imageService;

        public HomeController(IClusterClient orleansClient, IImageService imageService) 
        {
            this.orleansClient = orleansClient;
            this.imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("/save")]
        public async Task<IActionResult> Save(TodoModel model)
        {
            if(ModelState.IsValid) 
            {
                var uploadItem = await imageService.Prepare(model.FormFile);
                var todo = new TodoItem(Guid.NewGuid(), model.Name, DateTime.UtcNow);
                var todoGrain = orleansClient.GetGrain<ITodo>(Constants.TodoKey);
                await todoGrain.SetAsync(todo, uploadItem);
                return Ok(model);
            } 
            else 
            {
                var list = new List<string>();
                foreach (var modelStateVal in ViewData.ModelState.Values)
                {
                    list.AddRange(modelStateVal.Errors.Select(error => error.ErrorMessage));
                }
                return Json(new { status = "error", errors = list });
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
