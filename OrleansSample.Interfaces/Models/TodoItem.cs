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

        public string ImageUrl {get; set;}

        public TodoItem(Guid key, string title, DateTime time, string imageUrl = null) 
        {
            this.Key = key;
            this.Title = title;
            this.Timestamp = time;
            this.ImageUrl = imageUrl;
        }        
    }

    public class TodoImageUpload
    {
        public byte[] ImageData {get;}
        public string ImageName {get;}

        public TodoImageUpload(string name, byte[] data)
        {
            ImageName = name;
            ImageData = data;
        }
    }
}