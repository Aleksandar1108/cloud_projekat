using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Infrastructure.Common.Options
{
    public class SQLServerOptions
    {
        public string Server { get; set; } = string.Empty;

        public string Database { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


    }
}
