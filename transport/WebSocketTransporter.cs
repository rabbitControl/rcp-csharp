#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel.Composition;
using System.Windows.Forms;

using Fleck;
using VVVV.Core.Logging;
#endregion usings

namespace RCP
{
    public class WebsocketServerTransporter: IServerTransporter
    {
        private WebSocketServer FServer;
        private List<IWebSocketConnection> FAllSockets = new List<IWebSocketConnection>();

        public WebsocketServerTransporter(string remoteHost, int port)
        {
            FServer = new WebSocketServer("ws://" + remoteHost + ":" + port.ToString());
            FServer.Start(socket => {
                    socket.OnOpen = () =>
                    {
                        Console.WriteLine("Open!");
                        FAllSockets.Add(socket);
                    };

                    socket.OnClose = () =>
                    {
                        Console.WriteLine("Close!");
                        FAllSockets.Remove(socket);
                    };

                    socket.OnMessage = message =>
                    {
                        if (message.Length > 0 && Received != null)
                            Received(Encoding.UTF8.GetBytes(message));
                        //FAllSockets.ToList().ForEach(s => s.Send("Echo: " + message));
                    };

                    socket.OnBinary = bytes =>
                    {
                        if (bytes.Length > 0 && Received != null)
                            Received(bytes);
                    };
                });
        }

        public void Dispose()
        {
            if (FServer != null)
            {
//				FAllSockets.ToList().ForEach(s => s.Dispose());
                    FAllSockets.Clear();
                    FServer.Dispose();
            }
        }

        public void Send(byte[] bytes)
        {
            //send to all clients
            FAllSockets.ToList().ForEach(s => s.Send(bytes));
        }

        public Action<byte[]> Received {get; set;}

        private void ListenToUDP()
        {
//			while(FListening)
//			{
//				try
//				{
//					IPEndPoint ipEndPoint = null;
//					var bytes = FUDPReceiver.Receive(ref ipEndPoint);
//
//					if (bytes.Length > 0 && Received != null)
//						Received(bytes);
//				}
//				catch (Exception e)
//				{
//					//MessageBox.Show(e.Message);
//				}
//			}
        }
    }
}
