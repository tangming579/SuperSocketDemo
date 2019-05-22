using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class MyReceiveFilter : FixedHeaderReceiveFilter<MyPackageInfo>
    {
        public MyReceiveFilter() : base(4)
        {

        }

        public override MyPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] header = bufferStream.Buffers[0].ToArray();
            byte[] bodyBuffer = bufferStream.Buffers[1].ToArray();
            var package = new MyPackageInfo(header, bodyBuffer);
            return package;
        }

        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            ArraySegment<byte> buffers = bufferStream.Buffers[0];
            byte[] array = buffers.ToArray();
            int bodyLength = array[length - 2] * 256 + array[length - 1];
            return bodyLength;
        }
    }
}
