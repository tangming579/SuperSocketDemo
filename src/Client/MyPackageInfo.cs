using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// 消息体
    /// </summary>
    public class MyPackageInfo : IPackageInfo
    {
        public MyPackageInfo(byte[] header, byte[] body)
        {
            Key = ((header[0] * 256) + header[1]).ToString();
            Body = System.Text.Encoding.UTF8.GetString(body, 0, body.Length);
            IsHeart = string.Equals("2", Key);
        }

        public string Key { get; set; }

        public bool IsHeart { get; set; }

        public string Body { get; set; }
    }
}
