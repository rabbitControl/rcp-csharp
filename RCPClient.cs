using System;
using System.IO;
using System.Collections.Generic;

using RCP.Protocol;
using Kaitai;
using RCP.Parameter;
using RCP.Exceptions;
using System.Numerics;

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

            base.Dispose();
		}
		
        public Action<IParameter> ParameterAdded;
        public Action<IParameter> ParameterRemoved;

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
            foreach (var id in FDirtyParamIds)
                SendPacket(Pack(RcpTypes.Command.Update, FParams[id]));

            base.Update();
        }

        internal static IParameter CreateParameter(Int16 id, RcpTypes.Datatype datatype, IParameterManager manager)
        {
            var typeDefinition = TypeDefinition.Create(datatype);
            return typeDefinition.CreateParameter(id, manager);
        }

        internal static IParameter CreateArrayParameter(Int16 id, RcpTypes.Datatype elementType, IParameterManager manager)
        {
            var elementTypeDefinition = TypeDefinition.Create(elementType);
            var arrayTypeDefinition = elementTypeDefinition.CreateArray(new int[1]);
            return arrayTypeDefinition.CreateParameter(id, manager);
        }

        internal static IParameter CreateRangeParameter(Int16 id, RcpTypes.Datatype elementType, IParameterManager manager)
        {
            var elementTypeDefinition = TypeDefinition.Create(elementType);
            var rangeTypeDefinition = elementTypeDefinition.CreateRange();
            return rangeTypeDefinition.CreateParameter(id, manager);
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
            
            //during package parsing temp-parameters are created with _this as manager
            //and the parameters are immediately set dirty which is not correct
            //therefore clear the dirty params here: 
            FDirtyParamIds.Clear();
			//Logger.Log(LogType.Debug, packet.Command.ToString());
			
			switch (packet.Command)
			{
				case RcpTypes.Command.Update:
                    if (FParams.ContainsKey(packet.Data.Id))
                        (FParams[packet.Data.Id] as Parameter.Parameter).CopyFrom(packet.Data);
                    else
                    {
                        FParams.Add(packet.Data.Id, packet.Data);
                        //inform the application
                        ParameterAdded?.Invoke(packet.Data);
                    }
                    
				    break;

                case RcpTypes.Command.Updatevalue:
                    //TODO: actually only set the parameters value
                    if (FParams.ContainsKey(packet.Data.Id))
                        FParams.Remove(packet.Data.Id);
                    FParams.Add(packet.Data.Id, packet.Data);
                    //inform the application
                    break;

                case RcpTypes.Command.Remove:
                    FParams.Remove(packet.Data.Id);
				    //inform the application
				    ParameterRemoved?.Invoke(packet.Data);
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