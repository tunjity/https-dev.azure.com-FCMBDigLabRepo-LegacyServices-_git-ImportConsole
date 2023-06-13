using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportConsole.CyberHackImp
{
    public class RequestDto
    {
        public string AppID { get; set; }

        public string Safe { get; set; }

        public string Folder { get; set; }

        public string Object { get; set; }

        public string Reason { get; set; }

        public int Port { get; set; }
        public int TimeOut { get;  set; }
    }
}
