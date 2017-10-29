#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel.Composition;
using System.Windows.Forms;

using VVVV.Core.Logging;
#endregion usings

namespace RCP
{
	public class UDPServerTransporter: IServerTransporter
	{
		private UdpClient FUDPSender;
		private UdpClient FUDPReceiver;
		private Thread FThread;
		private bool FListening;
		
		public UDPServerTransporter(string remoteHost, int remotePort, int localPort)
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
			//send to all clients
			FUDPSender.Send(bytes, bytes.Length);
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
				catch (Exception)
				{
//					MessageBox.Show(e.Message);
				}
			}
		}
	}
}