using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Runtime;

namespace OrleansSample.Grains
{
    public class GrainObserverManager<T> : IEnumerable<T> where T : IAddressable
    {
        private readonly Dictionary<T, DateTime> observers = new Dictionary<T, DateTime>();

        public GrainObserverManager()
        {
        this.GetDateTime = () => DateTime.UtcNow;
        }
        public Func<DateTime> GetDateTime { get; set; }
        public TimeSpan ExpirationDuration { get; set; } = TimeSpan.FromMinutes(1);
        public int Count => this.observers.Count;        

        public void Clear()
        {
            this.observers.Clear();
        }
        public void Subscribe(T observer)
        {
            // Add or update the subscription.
            this.observers[observer] = this.GetDateTime();
        }
        public void Unsubscribe(T observer)
        {
            this.observers.Remove(observer);
        }
        public async Task Notify(Func<T, Task> notification, Func<T, bool> predicate = null)
        {
            var now = this.GetDateTime();
            var defunct = default(List<T>);
            foreach (var observer in this.observers)
            {
                if (observer.Value + this.ExpirationDuration < now)
                {
                    // Expired observers will be removed.
                    defunct = defunct ?? new List<T>();
                    defunct.Add(observer.Key);
                    continue;
                }
                // Skip observers which don't match the provided predicate.
                if (predicate != null && !predicate(observer.Key))
                {
                    continue;
                }
                try
                {
                    await notification(observer.Key);
                }
                catch (Exception)
                {
                    // Failing observers are considered defunct and will be removed..
                    defunct = defunct ?? new List<T>();
                    defunct.Add(observer.Key);
                }
            }
            // Remove defunct observers.
            if (defunct != default(List<T>))
            {
                foreach (var observer in defunct)
                {
                    this.observers.Remove(observer);
                }
            }
        }
        public void Notify(Action<T> notification, Func<T, bool> predicate = null)
        {
            var now = this.GetDateTime();
            var defunct = default(List<T>);
            foreach (var observer in this.observers)
            {
                if (observer.Value + this.ExpirationDuration < now)
                {
                    // Expired observers will be removed.
                    defunct = defunct ?? new List<T>();
                    defunct.Add(observer.Key);
                    continue;
                }
                // Skip observers which don't match the provided predicate.
                if (predicate != null && !predicate(observer.Key))
                {
                    continue;
                }
                try
                {
                    notification(observer.Key);
                }
                catch (Exception)
                {
                    // Failing observers are considered defunct and will be removed..
                    defunct = defunct ?? new List<T>();
                    defunct.Add(observer.Key);
                }
            }
            // Remove defunct observers.
            if (defunct != default(List<T>))
            {
                foreach (var observer in defunct)
                {
                    this.observers.Remove(observer);
                }
            }
        }
        public void ClearExpired()
        {
            var now = this.GetDateTime();
            var defunct = default(List<T>);
            foreach (var observer in this.observers)
            {
                if (observer.Value + this.ExpirationDuration < now)
                {
                    // Expired observers will be removed.
                    defunct = defunct ?? new List<T>();
                    defunct.Add(observer.Key);
                }
            }
            // Remove defunct observers.
            if (defunct != default(List<T>))
            {
                foreach (var observer in defunct)
                {
                    this.observers.Remove(observer);
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.observers.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.observers.Keys.GetEnumerator();
        }
    }
}