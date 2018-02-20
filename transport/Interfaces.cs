using System;

namespace RCP
{
	public interface IServerTransporter: IDisposable
	{
        void Bind(string host, int port);
        void Unbind();
        int ConnectionCount { get; }
		void SendToAll(byte[] bytes, object exceptId);
        void SendToOne(byte[] bytes, object id);
        Action<byte[], object> Received {get; set;}
	}
	
	public interface IClientTransporter: IDisposable
	{
        void Connect(string host, int port);
        void Disconnect();
        bool IsConnected { get; }
		void Send(byte[] bytes);

        Action Connected { get; set; }
        Action Disconnected { get; set; }
        Action<byte[]> Received {get; set;}
    }
}