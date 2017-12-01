using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

//using VVVV.Core.Logging;
using RCP.Model;
using Kaitai;
using System.Windows.Forms;

namespace RCP
{
    public class RabbitServer: Base 
	{
		private List<IServerTransporter> FTransporters = new List<IServerTransporter>();
		Dictionary<uint, IParameter> FParams = new Dictionary<uint, IParameter>();
		
		public Action<IParameter> ParameterUpdated;
		
		public uint[] IDs 
		{
			get { return FParams.Keys.ToArray(); } 
		}
		
		public IParameter GetParameter(uint id)
		{
			return FParams[id];
		}
		
		public override void Dispose()
		{
			foreach (var transporter in FTransporters)
				transporter.Dispose();
			
			FTransporters.Clear();
		}
		
		public bool AddParameter(IParameter parameter)
		{
			var result = false;
			if (!FParams.ContainsKey(parameter.Id))
			{
				FParams.Add(parameter.Id, parameter);
				result = true;
			}
			
			//dispatch to all clients
			SendToMultiple(Pack(RcpTypes.Command.Add, parameter));			
			//Logger.Log(LogType.Debug, "Server sent: Add Id: " + parameter.Id);
			
			return result;
		}
		
		public bool UpdateParameter(IParameter parameter)
		{
			//Logger.Log(LogType.Debug, "Server Update: " + parameter.Id);
			
			var result = false;
			if (FParams.ContainsKey(parameter.Id))
				FParams.Remove(parameter.Id);
			
			FParams.Add(parameter.Id, parameter);
			result = true;
			//Logger.Log(LogType.Debug, "Server sending..");
			//dispatch to all clients
			SendToMultiple(Pack(RcpTypes.Command.Update, parameter));
			//Logger.Log(LogType.Debug, "Server sent: Update");
			
			return result;
		}
		
		public bool RemoveParameter(uint id)
		{
			var param = FParams[id];
			var result = FParams.Remove(id);
			
			//dispatch to all clients
			SendToMultiple(Pack(RcpTypes.Command.Remove, param));
			//Logger.Log(LogType.Debug, "Server sent: Remove Id: " + id);
			
			return result;
		}
		
		#region Transporter
		public void AddTransporter(IServerTransporter transporter)
		{
			if (!FTransporters.Contains(transporter))
			{
				transporter.Received = ReceiveFromClientCB;
				FTransporters.Add(transporter);
			}
		}
		
		public void RemoveTransporter(IServerTransporter transporter)
		{
			if (FTransporters.Contains(transporter))
				FTransporters.Remove(transporter);
		}
		
		void ReceiveFromClientCB(byte[] bytes, IServerTransporter senderTransporter, string senderClient)
		{
			try
            {
			    var packet = Packet.Parse(new KaitaiStream(bytes));
				//MessageBox.Show(packet.Command.ToString());
		        switch (packet.Command)
		        {
			        case RcpTypes.Command.Update:
				        if (ParameterUpdated != null)
					        ParameterUpdated(packet.Data);
				        SendToMultiple(packet, senderClient);
				        break;
				
			        case RcpTypes.Command.Initialize:
				        //client requests all parameters
				        foreach (var param in FParams.Values)
					        SendToOne(Pack(RcpTypes.Command.Add, param), senderTransporter, senderClient);
				        break;
		        }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
		
		void SendToMultiple(Packet packet, string except = null)
		{
			using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                packet.Write(writer);
                var bytes = stream.ToArray();
            	foreach (var transporter in FTransporters)
					transporter.SendToAll(bytes, except);
            }
		}
		
		void SendToOne(Packet packet, IServerTransporter target, string client)
		{
			using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                packet.Write(writer);
                var bytes = stream.ToArray();
            	target.SendToOne(bytes, client);
            }
		}
		#endregion
	}
}