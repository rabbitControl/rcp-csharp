using System;
using System.IO;

using Kaitai;
using RCP.Protocol;
using RCP.Types;
using RCP.Parameters;

namespace RCP
{
    public class RCPClient: ClientServerBase
	{
		private IClientTransporter FTransporter;

        public RCPClient()
            : base()
        { }

        public RCPClient(IClientTransporter transporter)
            : this()
        {
            SetTransporter(transporter);
        }

		public override void Dispose()
		{
			if (FTransporter != null)
                FTransporter.Dispose();
		}

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

        public override void Update()
        {
            foreach (var parameter in FParams.Values)
                if (parameter.IsDirty)
                    SendPacket(Pack(RcpTypes.Command.Update, parameter));
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
			var packet = Packet.Parse(new KaitaiStream(bytes), this);
            var parameter = packet.Data;
            var id = parameter.Id;

			//Logger.Log(LogType.Debug, packet.Command.ToString());
			switch (packet.Command)
			{
				case RcpTypes.Command.Update:

                    if (!FParams.ContainsKey(id))
                        AddParameter(parameter);
                    else
                        parameter.RaiseEvents();
				    break;

                case RcpTypes.Command.Updatevalue:
                    if (FParams.ContainsKey(id))
                        parameter.RaiseEvents();
                    break;

                case RcpTypes.Command.Remove:
                    if (FParams.ContainsKey(id))
                        RemoveParameter(parameter);
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