using System;
using System.IO;
using System.Collections.Generic;

using RCP.Protocol;
using Kaitai;
using System.Windows.Forms;

namespace RCP
{
    public class RCPServer: Base 
	{
		List<IServerTransporter> FTransporters = new List<IServerTransporter>();
		Dictionary<int, IParameter> FParams = new Dictionary<int, IParameter>();

        public RCPServer()
        { }

        public RCPServer(IServerTransporter transporter)
        {
            AddTransporter(transporter);
        }

		public override void Dispose()
		{
            foreach (var transporter in FTransporters)
            {
                transporter.Received = null;
                transporter.Dispose();
            }
			
			FTransporters.Clear();

            base.Dispose();
		}
		
		public Action<IParameter> ParameterUpdated;

        public Action<IParameter> ParameterValueUpdated;

        public Action<Exception> ErrorLog;
		
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
		
		public bool RemoveParameter(int id)
		{
			var param = FParams[id];
			var result = FParams.Remove(id);
			
			//dispatch to all clients
			SendToMultiple(Pack(RcpTypes.Command.Remove, param));
			//Logger.Log(LogType.Debug, "Server sent: Remove Id: " + id);
			
			return result;
		}

        public IParameter GetParameter(int id)
		{
			return FParams[id];
		}
		
		#region Transporter
		public bool AddTransporter(IServerTransporter transporter)
		{
			if (!FTransporters.Contains(transporter))
			{
				transporter.Received = ReceiveFromClientCB;
				FTransporters.Add(transporter);
                return true;
			}
            
            return false;
		}
		
		public bool RemoveTransporter(IServerTransporter transporter)
		{
			if (FTransporters.Contains(transporter))
            {
				FTransporters.Remove(transporter);
                return true;
            }

            return false;
		}
		
		void ReceiveFromClientCB(byte[] bytes, object senderId)
		{
			try
            {
			    var packet = Packet.Parse(new KaitaiStream(bytes));
				//MessageBox.Show(packet.Command.ToString());
		        switch (packet.Command)
		        {
			        case RcpTypes.Command.Update:
                        if (FParams.ContainsKey(packet.Data.Id))
                            FParams.Remove(packet.Data.Id);
                        FParams.Add(packet.Data.Id, packet.Data);
                        ParameterUpdated?.Invoke(packet.Data);
				        SendToMultiple(packet, senderId);
				        break;

                    case RcpTypes.Command.Updatevalue:
                        if (FParams.ContainsKey(packet.Data.Id))
                            FParams.Remove(packet.Data.Id);
                        FParams.Add(packet.Data.Id, packet.Data);
                        ParameterValueUpdated?.Invoke(packet.Data);
                        SendToMultiple(packet, senderId);
                        break;

                    case RcpTypes.Command.Initialize:
				        //client requests all parameters
				        foreach (var param in FParams.Values)
					        SendToOne(Pack(RcpTypes.Command.Add, param), senderId);
		        	
		        	MessageBox.Show("sent init");
				        break;
		        }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
		
		void SendToMultiple(Packet packet, object exceptClientId = null)
		{
			using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                packet.Write(writer);
                var bytes = stream.ToArray();
            	foreach (var transporter in FTransporters)
					transporter.SendToAll(bytes, exceptClientId);
            }
		}
		
		void SendToOne(Packet packet, object clientId)
		{
			using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                packet.Write(writer);
                var bytes = stream.ToArray();
                foreach (var transporter in FTransporters)
                    transporter.SendToOne(bytes, clientId);
            }
		}
		#endregion
	}
}