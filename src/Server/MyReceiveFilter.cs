using SuperSocket.Facility.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //数据格式：
    //  -------+----------+------------------------------------------------------+
    //  0001   | 0010     |  4C36 3150 2D43 4D2B 4C30 3643 5055 2D43 4D2B 4C 4A  |
    //  固定头 | 数据长度 |  数据                                                |
    //         |          |                                                      |
    //  -------+----------+------------------------------------------------------+

    public class MyReceiveFilter : FixedHeaderReceiveFilter<MyRequestInfo>
    {
        //前四个字节为包头长度（headerSize）
        public MyReceiveFilter() : base(4)
        {

        }

        //解析消息中长度
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

            var info = new MyRequestInfo(header.ToArray(), body);
            return info;
        }
    }
}
