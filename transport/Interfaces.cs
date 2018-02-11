using System;

namespace RCP
{
	public interface IServerTransporter: IDisposable
	{
		void SendToAll(byte[] bytes, object exceptId);
        void SendToOne(byte[] bytes, object id);
        Action<byte[], object> Received {get; set;}
	}
	
	public interface IClientTransporter: IDisposable
	{
		void Send(byte[] bytes);
		Action<byte[]> Received {get; set;}
	}
}