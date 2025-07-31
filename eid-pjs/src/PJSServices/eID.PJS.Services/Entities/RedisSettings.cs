using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.Entities
{
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = "localhost:6379,password=passw@rd";

    }
}
