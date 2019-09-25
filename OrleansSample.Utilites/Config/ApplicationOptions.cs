using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansSample.Utilites.Config
{
    public class ApplicationOptions
    {
        public string OrleansConnectionString { get; set; }
        public string OrleansInvariant { get; set; }
        public string ClusterId { get; set; }
        public string ServiceId { get; set; }
    }
}
