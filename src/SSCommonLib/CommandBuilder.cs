using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSCommonLib
{
    public class CommandBuilder
    {
        public const Int16 HeartMsg = 01;
        public const Int16 HeaderHeart = 02;

        public static byte[] BuildMsgCmd(string msg)
        {
            var arr = new List<byte> { };
            var byteHeader = BitConverter.GetBytes(HeartMsg);
            var byteMsg = Encoding.UTF8.GetBytes(msg);
            var length = byteMsg.Length;

            var bodyLengthA = (byte)(length / 256);
            var bodyLengthB = (byte)length;
            arr.AddRange((byteHeader));
            arr.Add(bodyLengthA);
            arr.Add(bodyLengthB);
            arr.AddRange(byteMsg);
            return arr.ToArray();
        }
        public static byte[] BuildHeartCmd()
        {
            var byteMsg = Encoding.UTF8.GetBytes(DateTime.Now.ToString("1ssfff"));
            var arr = new List<byte> { };
            var byteHeader = BitConverter.GetBytes(HeaderHeart);
            var length = byteMsg.Length;

            var bodyLengthA = (byte)(length / 256);
            var bodyLengthB = (byte)length;
            arr.AddRange((byteHeader));
            arr.Add(bodyLengthA);
            arr.Add(bodyLengthB);
            arr.AddRange(byteMsg);
            return arr.ToArray();
        }
    }
}
