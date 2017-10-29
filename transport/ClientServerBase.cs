using System;
using System.Runtime.Serialization;
using VVVV.Core.Logging;

using RCP.Model;

namespace RCP
{
    public abstract class Base: IDisposable
	{
		public ILogger Logger { get; set; }
		
		protected Packet Pack(RcpTypes.Command command, dynamic parameter)
		{
			var packet = new Packet(command);
			packet.Data = parameter;
			
			return packet;
		}
		
		protected Packet Pack(RcpTypes.Command command, uint id)
		{
			var packet = new Packet(command);
			//packet.Data = new Parameter<T>(id, null);
			
			return packet;
		}
		
		protected Packet Pack(RcpTypes.Command command)
		{
			var packet = new Packet(command);
			
			return packet;
		}
		
		public virtual void Dispose()
		{
			Logger = null;
		}
	}
}