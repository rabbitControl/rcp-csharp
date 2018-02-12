using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

//using VVVV.Core.Logging;
using System.Windows.Forms;

namespace RCP.Transporter
{
    public abstract class UDPTransporter
    {
        protected UdpClient FUDPSender;
        private Thread FThread;
        private int FListeningPort;

        public UDPTransporter(string remoteHost, int remotePort, int listeningPort)
        {
            SetRemoteHostAndPort(remoteHost, remotePort);
            FListeningPort = listeningPort;
            StartListening();
        }

        private void StartListening()
        {
            var uiContext = SynchronizationContext.Current;

            Task.Run(() =>
            {
                FThread = Thread.CurrentThread;
                using (var udpClient = new UdpClient(FListeningPort))
                {
                    while (true)
                    {
                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var bytes = udpClient.Receive(ref remoteEndPoint);

                        if (bytes.Length > 0)
                            uiContext.Post((b) => OnReceived(b as byte[]), bytes);
                    }
                }
            });
        }

        private void StopListening()
        {
            if (FThread != null)
            {
                FThread.Abort();
                FThread = null;
            }
        }

        protected abstract void OnReceived(byte[] bytes);

        public void Dispose()
        {
            StopListening();

            if (FUDPSender != null)
            {
                FUDPSender.Close();
                FUDPSender.Dispose();
            }
        }

        public void SetRemoteHostAndPort(string host, int port)
        {
            if (FUDPSender != null)
            {
                FUDPSender.Close();
                FUDPSender.Dispose();
            }

            FUDPSender = new UdpClient(host, port);
        }

        public void SetListeningPort(int port)
        {
            StopListening();
            FListeningPort = port;
            StartListening();
        }
    }

    public class UDPServerTransporter: UDPTransporter, IServerTransporter
	{
        public Action<byte[], Object> Received { get; set; }

        public UDPServerTransporter(string remoteHost, int remotePort, int listeningPort):
            base(remoteHost, remotePort, listeningPort)
		{ }

        protected override void OnReceived(byte[] bytes)
        {
            Received?.Invoke(bytes, this);
        }
		
		public void SendToAll(byte[] bytes, object exceptId)
		{
            FUDPSender.Send(bytes, bytes.Length);
		}

        public void SendToOne(byte[] bytes, object id)
        {
            if (id == this)
                FUDPSender.Send(bytes, bytes.Length);
        }
	}
	
	public class UDPClientTransporter: UDPTransporter, IClientTransporter
	{
		public Action<byte[]> Received {get; set;}

        protected override void OnReceived(byte[] bytes)
        {
            Received?.Invoke(bytes);
        }

        public UDPClientTransporter(string remoteHost, int remotePort, int listeningPort) :
            base(remoteHost, remotePort, listeningPort)
        { }
		
		public void Send(byte[] bytes)
		{
			var r = FUDPSender.Send(bytes, bytes.Length);
		}
	}
}