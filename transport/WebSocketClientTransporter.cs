using System;
using System.Text;
using System.Threading;
using WebSocket4Net;

namespace RCP.Transporter
{
    public class WebsocketClientTransporter: IClientTransporter
    {
        private WebSocket FClient;
        private SynchronizationContext FContext;

        public Action<byte[]> Received { get; set; }
        public Action Connected { get; set; }
        public Action Disconnected { get; set; }

        public bool IsConnected => FClient?.Handshaked ?? false;

        public WebsocketClientTransporter(string remoteHost, int port)
        {
            FContext = SynchronizationContext.Current;

            CreateClient(remoteHost, port);
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
                FClient.Opened -= FClient_Opened;
                FClient.Closed -= FClient_Closed;
                FClient.MessageReceived -= FClient_MessageReceived;
                FClient.Dispose();
            }
        }

        private void CreateClient(string remoteHost, int port)
        {
            FClient = new WebSocket("ws://" + remoteHost + ":" + port.ToString());
            FClient.MessageReceived += FClient_MessageReceived;
            FClient.Opened += FClient_Opened;
            FClient.Opened += FClient_Closed;
        }

        public void SetRemoteHostAndPort(string remoteHost, int port)
        {
            var wasConnected = FClient?.Handshaked ?? false;

            DestroyClient();
            CreateClient(remoteHost, port);

            if (wasConnected)
                Connect();
        }

        public void Connect()
        {
            //here we could try to connect until we're connected
            //and try again when the connection gets lost
            FClient.Open();
        }

        public void Disconnect()
        {
            FClient.Close();
        }

        private void FClient_Opened(object sender, EventArgs e)
        {
            FContext.Post((b) => Connected?.Invoke(), null);
        }

        private void FClient_Closed(object sender, EventArgs e)
        {
            FContext.Post((b) => Disconnected?.Invoke(), null);
        }

        private void FClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.Length > 0)
            {
                //NOTE: encoding shit here only because Websocket4Net doesn't receive binary
                //beware: encoding obviously needs to be in sync with what server sends
                //setting both to UTF8 didn't work..
                var bytes = Encoding.Default.GetBytes(e.Message);
                FContext.Post((b) => Received?.Invoke(bytes), bytes);
            }
        }

        public void Send(byte[] bytes)
        {
            if (FClient != null)
                FClient.Send(bytes, 0, bytes.Length);
        }
    }
}
