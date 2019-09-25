using OrleansSample.Interfaces;
using System;
namespace OrleansSample.Client
{
    public class MessageObserver : IObserver
    {
        private readonly Action<string> _action;
        public MessageObserver(Action<string> caller) 
        {
            _action = caller;
        }
        public void ReceiveMessage(string message)
        {
            Console.WriteLine($"MessageObserver: {message}");
            _action(message);
        }
    }
}