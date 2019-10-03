using Orleans.Concurrency;
using System;

namespace OrleansSample.Interfaces.Models
{
    [Immutable]
    public class TodoNotification
    {
        public TodoNotification(NotificationType type)
        {
            this.NotificationType = type;
        }
        public TodoNotification(Guid key, TodoItem item, NotificationType type)
        {
            this.Key = key;
            this.Item = item;
            this.NotificationType = type;
        }
        public Guid Key {get;}
        public TodoItem Item {get;}

        public NotificationType NotificationType {get;}
    }

    public enum NotificationType
    {
        Add,
        Remove,
        Clear
    }
}