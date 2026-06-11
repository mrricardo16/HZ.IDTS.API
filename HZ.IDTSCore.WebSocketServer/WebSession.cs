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
            // 2026-06-10优化：连接建立后不再主动发送欢迎文本，避免客户端把非业务消息按压缩数据/业务JSON解析后主动断开。
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
