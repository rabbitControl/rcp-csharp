using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fleck;
//using VVVV.Core.Logging;

namespace RCP.Transporter
{
    public class WebsocketServerTransporter: IServerTransporter
    {
        private WebSocketServer FServer;
        private Dictionary<string, IWebSocketConnection> FSockets = new Dictionary<string, IWebSocketConnection>();

    	public Action<byte[], object> Received {get; set;}
    	
        public WebsocketServerTransporter(string remoteHost, int port)
        {
            FServer = new WebSocketServer("ws://" + remoteHost + ":" + port.ToString());
            FServer.Start(socket => {
                    socket.OnOpen = () =>
                    {
                        Console.WriteLine("Open!");
                        FSockets.Add(socket.ConnectionInfo.Origin, socket);
                    };

                    socket.OnClose = () =>
                    {
                        Console.WriteLine("Close!");
                        FSockets.Remove(socket.ConnectionInfo.Origin);
                    };

                    socket.OnMessage = message =>
                    {
//                        if (message.Length > 0 && Received != null)
//                            Received(Encoding.UTF8.GetBytes(message), this);
                        //FSockets.ToList().ForEach(s => s.Send("Echo: " + message));
                    };

                    socket.OnBinary = bytes =>
                    {
                        if (bytes.Length > 0 && Received != null)
                            Received(bytes, socket.ConnectionInfo.Origin);
                    };
                });
        }

        public void Dispose()
        {
            if (FServer != null)
            {
//				FSockets.ToList().ForEach(s => s.Dispose());
                    FSockets.Clear();
                    FServer.Dispose();
            }
        }

        public void SendToAll(byte[] bytes, object exceptId)
        {
            FSockets.Keys.ToList().ForEach(k => {
            	if (k != (exceptId as string))
            		FSockets[k].Send(bytes);
            });
        }

        public void SendToOne(byte[] bytes, object id)
        {
            var key = id as string;
            IWebSocketConnection socket;
            if (key != null && FSockets.TryGetValue(key, out socket))
                socket.Send(bytes);
        }
    }
}
