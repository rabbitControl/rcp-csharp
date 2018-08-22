using System;
using System.IO;
using System.Collections.Generic;

using RCP.Protocol;
using Kaitai;
using RCP.Parameter;
using RCP.Exceptions;

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
            switch (datatype)
            {
                case RcpTypes.Datatype.Boolean:
                    return new BooleanParameter(id, manager);

                case RcpTypes.Datatype.Enum:
                    return new EnumParameter(id, manager);

                case RcpTypes.Datatype.Int32:
                    return new NumberParameter<int>(id, manager);

                case RcpTypes.Datatype.Float32:
                    return new NumberParameter<float>(id, manager);

                case RcpTypes.Datatype.String:
                    return new StringParameter(id, manager);

                case RcpTypes.Datatype.Uri:
                    return new UriParameter(id, manager);

                case RcpTypes.Datatype.Rgba:
                    return new RGBAParameter(id, manager);

                case RcpTypes.Datatype.Vector2f32:
                    return new NumberParameter<Vector2>(id, manager);

                case RcpTypes.Datatype.Vector3f32:
                    return new NumberParameter<Vector3>(id, manager);

                case RcpTypes.Datatype.Vector4f32:
                    return new NumberParameter<Vector4>(id, manager);

                case RcpTypes.Datatype.Group:
                    return new GroupParameter(id, manager);

                default: throw new RCPUnsupportedFeatureException();
                    //array
            }
        }

        internal static IParameter CreateArrayParameter(Int16 id, RcpTypes.Datatype elementType, IParameterManager manager)
        {
            switch (elementType)
            {
                case RcpTypes.Datatype.String:
                    return new StringArrayParameter(id, manager);

                case RcpTypes.Datatype.Int32:
                    return new NumberArrayParameter<int[], int>(id, manager);

                case RcpTypes.Datatype.Float32:
                    return new NumberArrayParameter<float[], float>(id, manager);

                case RcpTypes.Datatype.Vector2f32:
                    return new NumberArrayParameter<Vector2[], Vector2>(id, manager);

                case RcpTypes.Datatype.Vector3f32:
                    return new NumberArrayParameter<Vector3[], Vector3>(id, manager);

                case RcpTypes.Datatype.Boolean:
                    return new BooleanArrayParameter(id, manager);

                case RcpTypes.Datatype.Enum:
                    return new EnumArrayParameter(id, manager);

                case RcpTypes.Datatype.Rgba:
                    return new RGBAArrayParameter(id, manager);

                case RcpTypes.Datatype.Uri:
                    return new UriArrayParameter(id, manager);

                default: throw new RCPUnsupportedFeatureException();
            }
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