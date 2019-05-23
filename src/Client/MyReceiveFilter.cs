using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    //数据格式：
    //  -------+----------+------------------------------------------------------+
    //  0001   | 0010     |  4C36 3150 2D43 4D2B 4C30 3643 5055 2D43 4D2B 4C 4A  |
    //  固定头 | 数据长度 |  数据                                                |
    //         |          |                                                      |
    //  -------+----------+------------------------------------------------------+

    public class MyReceiveFilter : FixedHeaderReceiveFilter<MyPackageInfo>
    {
        //前四个字节为包头长度（headerSize）
        public MyReceiveFilter() : base(4)
        {

        }
        //解析收到的数据
        public override MyPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] header = bufferStream.Buffers[0].ToArray();
            byte[] bodyBuffer = bufferStream.Buffers[1].ToArray();
            var package = new MyPackageInfo(header, bodyBuffer);
            return package;
        }
        //解析消息中长度
        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            ArraySegment<byte> buffers = bufferStream.Buffers[0];
            byte[] array = buffers.ToArray();
            int bodyLength = array[length - 2] * 256 + array[length - 1];
            return bodyLength;
        }
    }
}
