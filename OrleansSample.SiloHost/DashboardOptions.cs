namespace OrleansSample.SiloHost
{
    public class DashboardOptions
    {
        public string Host {get;set;}
        public int Port {get;set;}
        public bool HostSelf {get;set;}
        public int CounterUpdateIntervalMs {get;set;}

        public string Username {get;set;}
        public string Password {get;set;}

    }
}

/**
.UseDashboard(options => {
    options.Username = "USERNAME";
    options.Password = "PASSWORD";
    options.Host = "*";
    options.Port = 8080;
    options.HostSelf = true;
    options.CounterUpdateIntervalMs = 1000;
  })

 */
