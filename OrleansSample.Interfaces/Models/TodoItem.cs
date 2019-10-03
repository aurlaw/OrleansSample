using System;
using Orleans.Concurrency;

namespace OrleansSample.Interfaces.Models
{
    [Immutable]
    public class TodoItem
    {
        public Guid Key { get; }
        public string Title { get; }
        public DateTime Timestamp { get; }

        public TodoItem(Guid key, string title, DateTime time) 
        {
            this.Key = key;
            this.Title = title;
            this.Timestamp = time;
        }        
    }
}