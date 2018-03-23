using System;
using System.IO;
using System.Collections.Generic;

using RCP.Protocol;
using Kaitai;

namespace RCP
{
    public class RCPClient: ClientServerBase
	{
		Dictionary<byte[], IParameter> FParams = new Dictionary<byte[], IParameter>(new StructuralEqualityComparer<byte[]>());
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
        public Action<byte[]> ParameterRemoved;

        public Action<Exception> OnError;
        public Action<RcpTypes.ClientStatus, string> StatusChanged;


        public void Connect(string host, int port)
        {
            if (FTransporter.IsConnected)
                FTransporter.Disconnect();
            FTransporter.Connect(host, port);
        }

        public void Disonnect()
        {
            FTransporter.Disconnect();
        }

        public void Initialize()
		{
            FParams.Clear();
            SendPacket(Pack(RcpTypes.Command.Initialize));
		}
		
		public void Update(dynamic param)
		{
			SendPacket(Pack(RcpTypes.Command.Update, param));
		}

        public IParameter GetParameter(byte[] id)
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
				//case RcpTypes.Command.Add:
    //                Log?.Invoke("received add");
    //                FParams.Add(packet.Data.Id, packet.Data);
				//    //inform the application
				//    ParameterAdded?.Invoke(packet.Data);
				//    break;
				
				case RcpTypes.Command.Update:
                    if (FParams.ContainsKey(packet.Data.Id))
                        FParams.Remove(packet.Data.Id);
                    FParams.Add(packet.Data.Id, packet.Data);
				    //inform the application
				    ParameterUpdated?.Invoke(packet.Data);
				    break;

                case RcpTypes.Command.Updatevalue:
                    if (FParams.ContainsKey(packet.Data.Id))
                        FParams.Remove(packet.Data.Id);
                    FParams.Add(packet.Data.Id, packet.Data);
                    //inform the application
                    ParameterValueUpdated?.Invoke(packet.Data);
                    break;

                case RcpTypes.Command.Remove:
                    FParams.Remove(packet.Data.Id);
				    //inform the application
				    ParameterRemoved?.Invoke(packet.Data.Id);
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