using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

//using VVVV.Core.Logging;
using System.Windows.Forms;

namespace RCP
{
	public class UDPServerTransporter: IServerTransporter
	{
		private UdpClient FUDPSender;
		private UdpClient FUDPReceiver;
		private Thread FThread;
		private bool FListening;
		
		public UDPServerTransporter(string remoteHost, int sendingPort, int listeningPort)
		{
			FUDPSender = new UdpClient(remoteHost, sendingPort);
			FUDPReceiver = new UdpClient(listeningPort);
			FListening = true;
			FThread = new Thread(new ThreadStart(ListenToUDP));
			FThread.Start();
		}
		
		public void Dispose()
		{
			if (FThread != null && FThread.IsAlive)
			{
				FListening = false;
				//FThread.Join();
			}

			if (FUDPSender != null)
			{
				FUDPSender.Close();
				FUDPSender.Dispose();
			}	
			
			if (FUDPReceiver != null)
			{
				FUDPReceiver.Close();
				FUDPReceiver.Dispose();
			}	
		}
		
		public void SendToAll(byte[] bytes, string except)
		{
            FUDPSender.Send(bytes, bytes.Length);
		}

        public void SendToOne(byte[] bytes, string client)
        {
            FUDPSender.Send(bytes, bytes.Length);
        }

        public Action<byte[], IServerTransporter, string> Received {get; set;}
		
		private void ListenToUDP()
		{
			while(FListening)
			{
				try
				{
					IPEndPoint ipEndPoint = null;
					var bytes = FUDPReceiver.Receive(ref ipEndPoint);
					
					if (bytes.Length > 0 && Received != null)
					{
						Received(bytes, this, "udp");
					}
				}
				catch (Exception)
				{
					//MessageBox.Show(e.Message);
				}
			}
		}
	}
	
	public class UDPClientTransporter: IClientTransporter
	{
		private UdpClient FUDPSender;
		private UdpClient FUDPReceiver;
		private Thread FThread;
		private bool FListening;
		
		public UDPClientTransporter(string remoteHost, int remotePort, int localPort)
		{
			FUDPSender = new UdpClient(remoteHost, remotePort);
			FUDPReceiver = new UdpClient(localPort);
			FListening = true;
			FThread = new Thread(new ThreadStart(ListenToUDP));
			FThread.Start();
		}
		
		public void Dispose()
		{
			if (FThread != null && FThread.IsAlive)
			{
				FListening = false;
				//FThread.Join();
			}

			if (FUDPSender != null)
			{
				FUDPSender.Close();
				FUDPSender.Dispose();
			}	
			
			if (FUDPReceiver != null)
			{
				FUDPReceiver.Close();
				FUDPReceiver.Dispose();
			}	
		}
		
		public void Send(byte[] bytes)
		{
			var r = FUDPSender.Send(bytes, bytes.Length);
		}
		
		public Action<byte[]> Received {get; set;}
		
		private void ListenToUDP()
		{
			while(FListening)
			{
				try
				{
					IPEndPoint ipEndPoint = null;
					var bytes = FUDPReceiver.Receive(ref ipEndPoint);
					
					if (bytes.Length > 0 && Received != null)
						Received(bytes);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message);
				}
			}
		}
	}
}