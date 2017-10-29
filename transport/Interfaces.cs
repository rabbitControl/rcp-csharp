using System;

namespace RCP
{
	public interface IServerTransporter: IDisposable
	{
		void Send(byte[] packet);
		Action<byte[]> Received {get; set;}
	}
	
	public interface IClientTransporter: IDisposable
	{
		void Send(byte[] packet);
		Action<byte[]> Received {get; set;}
	}
}