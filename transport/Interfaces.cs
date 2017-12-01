using System;

namespace RCP
{
	public interface IServerTransporter: IDisposable
	{
		void SendToAll(byte[] bytes, string exceptClient);
        void SendToOne(byte[] bytes, string client);
        Action<byte[], IServerTransporter, string> Received {get; set;}
	}
	
	public interface IClientTransporter: IDisposable
	{
		void Send(byte[] bytes);
		Action<byte[]> Received {get; set;}
	}
}