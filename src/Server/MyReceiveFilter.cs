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
        public MyReceiveFilter() : base(4)
        {

        }
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            var bodyLength = (int)header[offset + 2] * 256 + (int)header[offset + 3];
            return bodyLength;
        }

        protected override MyRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            if (bodyBuffer == null) return null;

            var body = bodyBuffer.Skip(offset).Take(length).ToArray();
            if (body.Length < 2) return null;
            var isReply = body.Length == 2;

            var totalBuffer = new List<byte>();
            totalBuffer.AddRange(header.Array);
            totalBuffer.AddRange(body);

            var info = new MyRequestInfo();
            return info;
        }
    }
}
