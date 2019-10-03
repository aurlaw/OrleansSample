using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using OrleansSample.Interfaces.Models;

namespace OrleansSample.Interfaces
{
    public interface ITodo : IGrainWithGuidKey
    {
         Task SetAsync(TodoItem item);
         Task ClearAsync();
         Task<IEnumerable<TodoItem>> GetAllAsync();
    }
}