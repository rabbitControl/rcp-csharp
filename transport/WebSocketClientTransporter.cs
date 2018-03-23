using System;
using System.Threading;
using WebSocketSharp;

namespace RCP.Transporter
{
    public class WebsocketClientTransporter: IClientTransporter
    {
        private WebSocket FClient;
        private SynchronizationContext FContext;

        public Action<byte[]> Received { get; set; }
        public Action Connected { get; set; }
        public Action Disconnected { get; set; }

        public bool IsConnected => FClient?.IsAlive ?? false;

        public WebsocketClientTransporter()
        {
            FContext = SynchronizationContext.Current;
        }

        public void Dispose()
        {
            DestroyClient();
        }

        private void DestroyClient()
        {
            if (FClient != null)
            {
                FClient.Close();
                FClient.OnOpen -= FClient_Opened;
                FClient.OnClose -= FClient_Closed;
                FClient.OnMessage -= FClient_MessageReceived;
                FClient.Close();
            }
        }

        private void CreateClient(string remoteHost, int port)
        {
            FClient = new WebSocket("ws://" + remoteHost + ":" + port.ToString());
            FClient.OnMessage += FClient_MessageReceived;
            FClient.OnOpen += FClient_Opened;
            FClient.OnClose += FClient_Closed;
            FClient.Connect();
        }

        public void Connect(string remoteHost, int port)
        {
            DestroyClient();
            CreateClient(remoteHost, port);
        }

        public void Disconnect()
        {
            DestroyClient();
        }

        private void FClient_Opened(object sender, EventArgs e)
        {
            FContext.Post((b) => Connected?.Invoke(), null);
        }

        private void FClient_Closed(object sender, EventArgs e)
        {
            FContext.Post((b) => Disconnected?.Invoke(), null);
        }

        private void FClient_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.IsBinary && e.RawData.Length > 0)
            {
                FContext.Post((b) => Received?.Invoke(e.RawData), e.RawData);
            }
        }

        public void Send(byte[] bytes)
        {
            if (FClient != null)
                FClient.Send(bytes);
        }
    }
}
