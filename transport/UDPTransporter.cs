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
        private bool FListening;
        private bool FUpdatePort;
        private int FListeningPort;

        public UDPTransporter(string remoteHost, int remotePort, int listeningPort)
        {
            SetRemoteHostAndPort(remoteHost, remotePort);

            FListeningPort = listeningPort;
            FListening = true;

            Task.Run(() =>
            {
                while (FListening)
                {
                    using (var udpClient = new UdpClient(FListeningPort))
                    {
                        FUpdatePort = false;

                        while (!FUpdatePort)
                        {
                            //IPEndPoint object will allow us to read datagrams sent from any source.
                            var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                            var bytes = udpClient.Receive(ref remoteEndPoint);

                            if (bytes.Length > 0)
                                OnReceived(bytes);
                        }
                    }
                }
            });
        }

        protected abstract void OnReceived(byte[] bytes);

        public void Dispose()
        {
            FListening = false;

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
            FListeningPort = port;
            FUpdatePort = true;
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