using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fleck;
using System.Threading;
//using VVVV.Core.Logging;

namespace RCP.Transporter
{
    public class WebsocketServerTransporter: IServerTransporter
    {
        private SynchronizationContext FContext;
        private WebSocketServer FServer;
        private Dictionary<Guid, IWebSocketConnection> FSockets = new Dictionary<Guid, IWebSocketConnection>();

    	public Action<byte[], object> Received {get; set;}
    	public int ConnectionCount => FSockets.Count;
    	
        public WebsocketServerTransporter(string remoteHost, int port)
        {
            FContext = SynchronizationContext.Current;

            CreateServer(remoteHost, port);
        }

        public void Dispose()
        {
            DestroyServer();
        }

        private void DestroyServer()
        {
            if (FServer != null)
            {
                FSockets.Clear();
                FServer.Dispose();
            }
        }

        private void CreateServer(string remoteHost, int port)
        {
            FServer = new WebSocketServer("ws://" + remoteHost + ":" + port.ToString());
            FServer.Start(socket => {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Open!");
                    FSockets.Add(socket.ConnectionInfo.Id, socket);
                };

                socket.OnClose = () =>
                {
                    Console.WriteLine("Close!");
                    FSockets.Remove(socket.ConnectionInfo.Id);
                };

                socket.OnMessage = message =>
                {
                    //this shouldn't be necessary
                    //if (message.Length > 0)
                    //    FContext.Post((m) => Received?.Invoke(Encoding.Default.GetBytes(m as string), socket.ConnectionInfo.Id), message);
                };

                socket.OnBinary = bytes =>
                {
                    if (bytes.Length > 0)
                        FContext.Post((b) => Received?.Invoke(b as byte[], socket.ConnectionInfo.Id), bytes);
                };
            });
        }

        public void SetRemoteHostAndPort(string remoteHost, int port)
        {
            DestroyServer();
            CreateServer(remoteHost, port);
        }

        public void SendToAll(byte[] bytes, object exceptId)
        {
            FSockets.Keys.ToList().ForEach(k => {
            	if (exceptId == null || k != (Guid)exceptId)
                    //TODO: better send bytes directly once receiver understands that
                    //FSockets[k].Send(bytes);
                    //UTF8 encoding didn't work for some reason
                    FSockets[k].Send(Encoding.Default.GetString(bytes));
            });
        }

        public void SendToOne(byte[] bytes, object id)
        {
            IWebSocketConnection socket;
            if (id != null && FSockets.TryGetValue((Guid)id, out socket))
                //TODO: better send bytes directly once receiver understands that
                //socket.Send(bytes);
                //UTF8 encoding didn't work for some reason
                socket.Send(Encoding.Default.GetString(bytes));
        }
    }
}
