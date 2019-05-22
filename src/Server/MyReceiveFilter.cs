using SuperSocket.Facility.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class MyReceiveFilter : FixedHeaderReceiveFilter<MyRequestInfo>
    {
        //前四个字节为包头和长度描述
        public MyReceiveFilter() : base(4)
        {

        }

        //解析消息中长度描述部分
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var bodyLength = (int)header[offset + 2] * 256 + (int)header[offset + 3];
            return bodyLength;
        }

        //解析收到的数据
        protected override MyRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            if (bodyBuffer == null) return null;

            var body = bodyBuffer.Skip(offset).Take(length).ToArray();

            var totalBuffer = new List<byte>();
            totalBuffer.AddRange(header.ToArray());
            totalBuffer.AddRange(body);

            var info = new MyRequestInfo(header.ToArray(), totalBuffer.ToArray());
            return info;
        }
    }
}
