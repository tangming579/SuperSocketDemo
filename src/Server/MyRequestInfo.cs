using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class MyRequestInfo : IRequestInfo
    {
        public string Key { get; set; }
    }
}
