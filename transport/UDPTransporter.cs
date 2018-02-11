using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

//using VVVV.Core.Logging;
using System.Windows.Forms;

namespace RCP.Transporter
{
	public class UDPServerTransporter: IServerTransporter
	{
		private UdpClient FUDPSender;
		private bool FListening;

        public Action<byte[], Object> Received { get; set; }

        public UDPServerTransporter(string remoteHost, int sendingPort, int listeningPort)
		{
			FUDPSender = new UdpClient(remoteHost, sendingPort);
			FListening = true;

            Task.Run(() =>
            {
                using (var udpClient = new UdpClient(listeningPort))
                {
                    while (FListening)
                    {
                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var bytes = udpClient.Receive(ref remoteEndPoint);

                        if (bytes.Length > 0 && Received != null)
                            Received(bytes, this);
                    }
                }
            });
        }
		
		public void Dispose()
		{
			FListening = false;

			if (FUDPSender != null)
			{
				FUDPSender.Close();
				FUDPSender.Dispose();
			}	
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
	
	public class UDPClientTransporter: IClientTransporter
	{
		private UdpClient FUDPSender;
		private bool FListening;
		
		public Action<byte[]> Received {get; set;}
		
		public UDPClientTransporter(string remoteHost, int remotePort, int localPort)
		{
			FUDPSender = new UdpClient(remoteHost, remotePort);
			FListening = true;

            Task.Run(() =>
            {
                using (var udpClient = new UdpClient(localPort))
                {
                    while (FListening)
                    {
                        //IPEndPoint object will allow us to read datagrams sent from any source.
                        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var bytes = udpClient.Receive(ref remoteEndPoint);

                        if (bytes.Length > 0 && Received != null)
                            Received(bytes);
                    }
                }
            });
		}
		
		public void Dispose()
		{
            FListening = false;

			if (FUDPSender != null)
			{
				FUDPSender.Close();
				FUDPSender.Dispose();
			}	
		}
		
		public void Send(byte[] bytes)
		{
			var r = FUDPSender.Send(bytes, bytes.Length);
		}
	}
}