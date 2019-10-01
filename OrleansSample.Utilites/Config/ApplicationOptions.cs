using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansSample.Utilites.Config
{
    public class ApplicationOptions
    {
        public string OrleansConnectionString { get; set; }
        public string ClusterId { get; set; }
        public string ServiceId { get; set; }

        public bool UseJson {get;set;}
        public StorageType StorageType {get;set;}

        public string AdoInvariant { get; set; }
        public string AzureTableName { get; set; }

    }

    public enum StorageType 
    {
        Ado  =0,
        AzureTable = 1
    }
}
