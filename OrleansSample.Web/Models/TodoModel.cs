using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OrleansSample.Web.Models
{
    public class TodoModel
    {
        [Required]
        public string Name {get;set;}

        public IFormFile FormFile {get; set;}
    }
}