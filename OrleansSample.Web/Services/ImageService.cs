using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrleansSample.Interfaces.Models;

namespace OrleansSample.Web.Services
{
    public class ImageService : IImageService
    {
        private ILogger logger;

        public ImageService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}");
        }


        public async Task<TodoImageUpload> Prepare(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                return new TodoImageUpload(file.FileName, stream.ToArray());   
            }
        }
    }
}