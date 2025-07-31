using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogSourceApp
{
    public class SystemIdProvider
    {
        private readonly string _systemId;
        private readonly string[] systems = {"eid-iscei",
                                    "eid-mpozei",
                                    "eid-pan",
                                    "eid-pg",
                                    "eid-pjs",
                                    "eid-pun",
                                    "eid-rei",
                                    "eid-ruei"};
        public SystemIdProvider()
        {
            var index = (new Random()).Next(0, 8);
            _systemId = systems[index];
        }
        public string SystemId
        {
            get
            {
                return _systemId;
            }
        }
    }
}
