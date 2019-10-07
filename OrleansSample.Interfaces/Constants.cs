using System;

namespace OrleansSample.Interfaces
{
    public class Constants
    {
        public const string StorageName = "OrleansStorage";

        public const string StreamProvider = "SMS";
        public const string StreamStorage = "PubSubStore";

        public const string StorageContainer = "orleans";
        public static Guid TodoKey => Guid.Parse("81b8c834-a7b3-4b3c-8f4e-3e37df924ff3"); 

    }
}