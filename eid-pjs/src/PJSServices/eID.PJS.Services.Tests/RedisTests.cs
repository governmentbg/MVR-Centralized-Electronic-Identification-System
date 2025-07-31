using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.Services.Entities;

using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using StackExchange.Redis.Profiling;

namespace eID.PJS.Services.Tests
{
    public class RedisTests
    {
        private RedisSettings _settings;
        public RedisTests() 
        {
            _settings = new RedisSettings();
        }


        public void PingTest()
        { 
            
        }
    }
}
