using HZ.CommonUtil.Helpers;
using SuperSocket.Channel;
using SuperSocket.WebSocket.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.WebSocketServer
{
    public class WebSession : WebSocketSession
    {
        protected override async ValueTask OnSessionConnectedAsync()
        {
            SessionInstance.Instance.AddSession(this);
            await base.SendAsync("Connection succeeded ok");
        }

        protected override ValueTask OnSessionClosedAsync(CloseEventArgs e)
        {
            SessionInstance.Instance.SessionRemove(this);
            return base.OnSessionClosedAsync(e);
        }

        public override async ValueTask SendAsync(string message)
        {
            await base.SendAsync(message);
        }
    }
}
