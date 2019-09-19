using Orleans;
using OrleansSample.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansSample.Grains
{
    public class HelloArchiveGrain : Grain<GreetingArchive>, IHelloArchive
    {
        public Task<IEnumerable<string>> GetGreetings()
        {
            return Task.FromResult<IEnumerable<string>>(State.Greetings);
        }

        public async Task<string> SayHello(string greeting)
        {
            State.Greetings.Add(greeting);

            await WriteStateAsync();

            return $"You said: '{greeting}', I say: Hello!";
        }
    }

    public class GreetingArchive
    {
        public List<string> Greetings { get; } = new List<string>();
    }
}
