using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OrleansSample.Interfaces.Models;

namespace OrleansSample.Web.Services
{
    public class ImageService : IImageService
    {
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