using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.models
{
    public class Store
    {
        public string Id { get; set; }
        public string StoreNumber { get; set; }
        public string StoreName { get; set; }
        public string TermCount { get; set; }
        public string Version { get; set; }
        public string State { get; set; }
        public string TaxRate { get; set; }
        public string GroupId { get; set; }
    }
}
