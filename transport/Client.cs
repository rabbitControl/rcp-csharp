using System;
using System.IO;
using System.Collections.Generic;
using VVVV.Core.Logging;

using RCP.Model;
using Kaitai;

namespace RCP
{
    public class Client: Base
	{
		public Dictionary<uint, dynamic> FParams = new Dictionary<uint, dynamic>();
		public Dictionary<uint, dynamic> Params
		{
			get { return FParams; }
		}
		public IEnumerable<string> Parameters 
		{
			get 
			{ 
				foreach (var param in FParams.Values)
				{
					yield return param.Id + ", "
						+ param.TypeDefinition.Datatype.ToString() + ", "
						+ param.Value.ToString() + ", "
						+ param.Label + ", "
						+ param.Description + ", "
						+ param.Order + ", "
						+ param.Parent + ", "
						//widget
						//userdata
						;
				}
			}
		}
		
		private IClientTransporter FTransporter;
		public IClientTransporter Transporter
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
		
		public override void Dispose()
		{
			if (FTransporter != null)
				FTransporter.Dispose();
		}
		
		public void Initialize()
		{
			FParams.Clear();
			SendPacket(Pack(RcpTypes.Command.Initialize));
//			Logger.Log(LogType.Debug, "Client sent Init");
		}
		
		public void Update(dynamic param)
		{
			SendPacket(Pack(RcpTypes.Command.Update, param));
//			Logger.Log(LogType.Debug, "Client sent Update");
		}
		
		public Action<uint> ParameterAdded;
		public Action<uint> ParameterUpdated;
		public Action<uint> ParameterRemoved;
		
		void ReceiveCB(byte[] bytes)
		{
			Logger.Log(LogType.Debug, "Client received: " + bytes.Length + "bytes");
			var packet = Packet.Parse(new KaitaiStream(bytes));
			Logger.Log(LogType.Debug, packet.Command.ToString());
			
			switch (packet.Command)
			{
				case RcpTypes.Command.Add:
				FParams.Add(packet.Data.Id, packet.Data);
				//inform the application
				if (ParameterAdded != null)
					ParameterAdded(packet.Data.Id);
				break;
				
				case RcpTypes.Command.Update:
				FParams.Remove(packet.Data.Id);
				FParams.Add(packet.Data.Id, packet.Data);
				//inform the application
				if (ParameterUpdated != null)
					ParameterUpdated(packet.Data.Id);
				break;
				
				case RcpTypes.Command.Remove:
				FParams.Remove(packet.Data.Id);
				//inform the application
				if (ParameterRemoved != null)
					ParameterRemoved(packet.Data.Id);
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
	}
}