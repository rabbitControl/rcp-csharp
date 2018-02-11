using System;
using System.IO;
using System.Collections.Generic;

using RCP.Protocol;
using Kaitai;

namespace RCP
{
    public class RCPClient: Base
	{
		Dictionary<int, IParameter> FParams = new Dictionary<int, IParameter>();
		private IClientTransporter FTransporter;

        public RCPClient()
        { }

        public RCPClient(IClientTransporter transporter)
        {
            SetTransporter(transporter);
        }

		public override void Dispose()
		{
			if (FTransporter != null)
                FTransporter.Dispose();

            base.Dispose();
		}
		
        public Action<IParameter> ParameterAdded;
        public Action<IParameter> ParameterUpdated;
        public Action<IParameter> ParameterValueUpdated;
        public Action<IParameter> ParameterRemoved;

        public Action<Exception> ErrorLog;
        public Action<RcpTypes.Status, string> StatusChanged;

        public void Initialize()
		{
			FParams.Clear();
            SendPacket(Pack(RcpTypes.Command.Initialize));
		}
		
		public void Update(dynamic param)
		{
			SendPacket(Pack(RcpTypes.Command.Update, param));
		}

        public IParameter GetParameter(int id)
        {
            return FParams[id];
        }

        #region Transporter
        public void SetTransporter(IClientTransporter transporter)
        {
            if (FTransporter != null)
            {
                FTransporter.Received = null;
                FTransporter.Dispose();
            }

            FTransporter = transporter;
            FTransporter.Received = ReceiveCB;
        }

        void ReceiveCB(byte[] bytes)
		{
			//Logger.Log(LogType.Debug, "Client received: " + bytes.Length + "bytes");
			var packet = Packet.Parse(new KaitaiStream(bytes));
			//Logger.Log(LogType.Debug, packet.Command.ToString());
			
			switch (packet.Command)
			{
				case RcpTypes.Command.Add:
				FParams.Add(packet.Data.Id, packet.Data);
				//inform the application
				if (ParameterAdded != null)
					ParameterAdded(packet.Data);
				break;
				
				case RcpTypes.Command.Update:
				FParams.Remove(packet.Data.Id);
				FParams.Add(packet.Data.Id, packet.Data);
				//inform the application
				if (ParameterUpdated != null)
					ParameterUpdated(packet.Data);
				break;
				
				case RcpTypes.Command.Remove:
				FParams.Remove(packet.Data.Id);
				//inform the application
				if (ParameterRemoved != null)
					ParameterRemoved(packet.Data);
				break;
			}
		}
		
		void SendPacket(Packet packet)
		{
			using (var stream = new MemoryStream())
			using (var writer = new BinaryWriter(stream))
			{
				packet.Write(writer);
				FTransporter.Send(stream.ToArray());
			}
		}
        #endregion
    }
}