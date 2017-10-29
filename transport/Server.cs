using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using VVVV.Core.Logging;
using RCP.Model;
using Kaitai;

namespace RCP
{
    public class Server: Base 
	{
		private IServerTransporter FTransporter;
		public IServerTransporter Transporter
		{ 
			get { return FTransporter; }
			
			set 
			{
				if (FTransporter != null)
					FTransporter.Dispose();
				
				FTransporter = value;	
				FTransporter.Received = ReceiveCB;
			}
		}
		
		public Action<IParameter> ParameterUpdated;
		
		Dictionary<uint, IParameter> FParams = new Dictionary<uint, IParameter>();
		
		public uint[] IDs 
		{
			get { return FParams.Keys.ToArray(); } 
		}
		
		public override void Dispose()
		{
			//remove all values?
			
			if (FTransporter != null)
				FTransporter.Dispose();
		}
		
		public bool AddParameter(IParameter parameter)
		{
			var result = false;
			if (!FParams.ContainsKey(parameter.Id))
			{
				FParams.Add(parameter.Id, parameter);
				result = true;
			}
			
			//dispatch to all clients via transporter
			SendPacket(Pack(RcpTypes.Command.Add, parameter));			
			Logger.Log(LogType.Debug, "Server sent: Add Id: " + parameter.Id);
			
			return result;
		}
		
		public bool UpdateParameter(IParameter parameter)
		{
			Logger.Log(LogType.Debug, "Server Update: " + parameter.Id);
			
			var result = false;
			if (FParams.ContainsKey(parameter.Id))
				FParams.Remove(parameter.Id);
			
			FParams.Add(parameter.Id, parameter);
			result = true;
			Logger.Log(LogType.Debug, "Server sending..");
			//dispatch to all clients via transporter
			SendPacket(Pack(RcpTypes.Command.Update, parameter));
			Logger.Log(LogType.Debug, "Server sent: Update");
			
			return result;
		}
		
		public bool RemoveParameter(uint id)
		{
			var param = FParams[id];
			var result = FParams.Remove(id);
			
			//dispatch to all clients via transporter
			SendPacket(Pack(RcpTypes.Command.Remove, param));
			Logger.Log(LogType.Debug, "Server sent: Remove Id: " + id);
			
			return result;
		}
		
		#region Transporter
		void ReceiveCB(byte[] bytes)
		{
			Logger.Log(LogType.Debug, "Server received packet from Client:");
			var packet = Packet.Parse(new KaitaiStream(bytes));
			Logger.Log(LogType.Debug, packet.Command.ToString());
			
			switch (packet.Command)
			{
				case RcpTypes.Command.Update:
					if (ParameterUpdated != null)
						ParameterUpdated(packet.Data);
					break;
				
				case RcpTypes.Command.Initialize:
					//client requests all parameters
					foreach (var param in FParams.Values)
						SendPacket(Pack(RcpTypes.Command.Add, param));
					break;
			}
		}
		
		void SendPacket(Packet packet)
		{
			using (var stream = new MemoryStream())
			using (var writer = new BinaryWriter(stream))
			{
				packet.Write(writer);
				Transporter.Send(stream.ToArray());
			}
		}
		#endregion
	}
}