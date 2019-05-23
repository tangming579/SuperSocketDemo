## 一个基于WPF + SuperSocket的TCP长连接小示例

SuperSocket 是一个轻量级, 跨平台而且可扩展的 .Net/Mono Socket 服务器程序框架。你无须了解如何使用 Socket, 如何维护 Socket 连接和 Socket 如何工作，我们可以有更多的时间用在业务逻辑上，SuperSocket有效的利用自己的协议解决粘包及各种事件通知机制。

GitHub地址：https://github.com/kerryjiang/SuperSocket

**实现功能：**

- 心跳检测
- 断线重连
- 粘包组包

**粘包**

一般所谓的TCP粘包是在一次接收数据不能完全地体现一个完整的消息数据。TCP通讯存在粘包的主要原因是TCP是以流的方式来处理数据，所以就会引发一次接收的数据无法满足消息的需要，导致粘包的存在。处理粘包的唯一方法就是制定应用层的数据通讯协议，通过协议来规范现有接收的数据是否满足消息数据的需要。

解决办法：

```
1. 消息定长：报文大小固定长度，不够空格补全，发送和接收方遵循相同的约定，这样即使粘包了通过接收方编程实现获取定长报文也能区分。
2. 包尾添加特殊分隔符：例如每条报文结束都添加回车换行符（例如FTP协议）或者指定特殊字符作为报文分隔符，接收方通过特殊分隔符切分报文区分。
3. 将消息分为消息头和消息体：消息头中包含表示信息的总长度（或者消息体长度）的字段
```

SuperSocket中的常用内置协议：

- TerminatorReceiveFilter：结束符协议
- CountSpliterReceiveFilter：固定数量分隔符协议
- FixedSizeReceiveFilter：固定请求大小的协议
- BeginEndMarkReceiveFilter：带起止符的协议
- FixedHeaderReceiveFilter：头部格式固定并且包含内容长度的协议

**SuperSocket解决粘包说明**

1. FixedHeaderReceiveFilter即为将消息分为消息头和消息体来解决粘包问题

```
//数据格式：
//  -------+----------+------------------------------------------------------+
//  0001   | 0010     |  5412 0234 0001 0543 06215 04312 06542               |
//  请求名  | 数据长度  |  数据                                                 |
//  -------+----------+------------------------------------------------------+
```

实现自定义的FixedHeaderReceiveFilter需要实现GetBodyLengthFromHeader与ResolveRequestInfo方法

```c#
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
```



2. BeginEndMarkReceiveFilter即为带起止符的协议

```
//数据格式：
//  -------+----------+------------------------------------------------------+
//  8787   |   4C36 3150 2D43 4D2B 4C30 3643 5055 2D43 4D2B 4C 4A   |  8989  |
//  固定头  |   数据                                                  |  固定尾 |
//  -------+----------+------------------------------------------------------+
```

```c#
public class AlgReceiveFilter : BeginEndMarkReceiveFilter<StringRequestInfo>
    {
        //开始和结束标记也可以是两个或两个以上的字节
        private readonly static byte[] BeginMark = new byte[] { (byte)'#' };
        private readonly static byte[] EndMark = new byte[] { (byte)'@' };

        public AlgReceiveFilter()
        : base(BeginMark, EndMark) //传入开始标记和结束标记
        {

        }

        protected override StringRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            //通过解析到的数据来构造请求实例，并返回
            var totalBuffer = readBuffer.Skip(offset).Take(length).ToArray();
            var str = System.Text.Encoding.UTF8.GetString(totalBuffer);
            var body = str.Trim('#', '@');
            return new StringRequestInfo("", body, null);
        }
    }
```

**另外**

更高级的应用请参考官方文档：http://docs.supersocket.net/

另外[DotNetty](https://github.com/tangming579/DotNettySample)是另一个比较好用的.Net通讯框架，也很推荐