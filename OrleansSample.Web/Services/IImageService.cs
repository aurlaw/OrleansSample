using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OrleansSample.Interfaces.Models;

namespace OrleansSample.Web.Services
{
    public interface IImageService
    {
         Task<TodoImageUpload> Prepare(IFormFile file); 
    }
}