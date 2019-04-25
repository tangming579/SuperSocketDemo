using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class MyServer : AppServer<MySession, MyRequestInfo>
    {
        public MyServer()
           : base(new DefaultReceiveFilterFactory<MyReceiveFilter, MyRequestInfo>())
        {
            this.NewSessionConnected += MyServer_NewSessionConnected;
            this.SessionClosed += MyServer_SessionClosed;
        }

        public MyServer(SessionHandler<MySession> NewSessionConnected, SessionHandler<MySession, CloseReason> SessionClosed)
            : base(new DefaultReceiveFilterFactory<MyReceiveFilter, MyRequestInfo>())
        {
            this.NewSessionConnected += NewSessionConnected;
            this.SessionClosed += SessionClosed;
        }

        protected override void OnStarted()
        {
            //启动成功
        }

        void MyServer_NewSessionConnected(MySession session)
        {
            //连接成功
        }

        void MyServer_SessionClosed(MySession session, CloseReason value)
        {

        }
    }
}
