using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSProcessor.CLI.DB
{
    public class MongoOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
