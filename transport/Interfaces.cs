using System;

namespace RCP
{
	public interface IServerTransporter: IDisposable
	{
		void Send(byte[] bytes);
		Action<byte[], IServerTransporter> Received {get; set;}
	}
	
	public interface IClientTransporter: IDisposable
	{
		void Send(byte[] bytes);
		Action<byte[]> Received {get; set;}
	}
}