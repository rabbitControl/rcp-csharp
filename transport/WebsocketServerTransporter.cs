using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatsonWebsocket;
using System.Threading;
//using VVVV.Core.Logging;

namespace RCP.Transporter
{
    public class WebsocketServerTransporter: IServerTransporter
    {
        private SynchronizationContext FContext;
        private WatsonWsServer FServer;

    	public Action<byte[], object> Received {get; set;}
    	public int ConnectionCount => FServer.ListClients().Count();
    	
        public WebsocketServerTransporter(string remoteHost, int port)
        {
            FContext = SynchronizationContext.Current;

            CreateServer(remoteHost, port);
        }

        public void Dispose()
        {
            Unbind();
        }

        private void CreateServer(string remoteHost, int port)
        {
            FServer = new WatsonWsServer(remoteHost, port, false);
            FServer.MessageReceived += FServer_MessageReceived;

            FServer.Start();
        }

        private void FServer_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Data.Count > 0)
                FContext.Post((b) => Received?.Invoke(b as byte[], e.IpPort), e.Data.Array);
        }

        public void Bind(string remoteHost, int port)
        {
            Unbind();
            CreateServer(remoteHost, port);
        }

        public void Unbind()
        {
            if (FServer != null)
            {
                foreach (var client in FServer.ListClients())
                    FServer.DisconnectClient(client);
                try
                {
                    FServer.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        public void SendToAll(byte[] bytes, object exceptId)
        {
            foreach (var client in FServer.ListClients())
                if (exceptId == null || client != (string)exceptId)
                    FServer.SendAsync(client, bytes);
        }

        public void SendToOne(byte[] bytes, object id)
        {
            if (id != null && FServer.ListClients().Contains((string)id))
                FServer.SendAsync((string)id, bytes);
        }
    }
}
