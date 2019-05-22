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
        public MyRequestInfo(byte[] header, byte[] body)
        {
            Body = System.Text.Encoding.UTF8.GetString(body, 0, body.Length);
        }
        public string Key { get; set; }

        public bool IsHeart { get; set; }

        public string Body { get; set; }
    }
}
