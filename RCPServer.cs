using System;
using System.IO;
using System.Collections.Generic;

using RCP.Protocol;
using Kaitai;
using System.Windows.Forms;
using System.Collections;
using System.Linq;
using RCP.Parameter;
using System.Numerics;

namespace RCP
{
    public class RCPServer: ClientServerBase 
	{
		List<IServerTransporter> FTransporters = new List<IServerTransporter>();
        Int16 FIdCounter = 1;

        public RCPServer()
            : base()
        { }
 
        public RCPServer(IServerTransporter transporter)
            : this()
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

        public IParameter CreateParameter(RcpTypes.Datatype datatype)
        {
            var param = ParameterFactory.CreateParameter(FIdCounter++, datatype, this);
            FParams.Add(param.Id, param);
            return param;
        }

        public NumberParameter<T> CreateNumberParameter<T>() where T: struct
        {
            INumberDefinition<T> definition = null;
            if (typeof(T) == typeof(float))
                definition = (INumberDefinition<T>)new Float32Definition();
            else if (typeof(T) == typeof(int))
                definition = (INumberDefinition<T>)new Integer32Definition();
            else if (typeof(T) == typeof(Vector2))
                definition = (INumberDefinition<T>)new Vector2f32Definition();
            else if (typeof(T) == typeof(Vector3))
                definition = (INumberDefinition<T>)new Vector3f32Definition();

            var param = new NumberParameter<T>(FIdCounter++, definition, this);
            FParams.Add(param.Id, param);
            return param;
        }

        public StringParameter CreateStringParameter()
        {
            var param = new StringParameter(FIdCounter++, this);
            FParams.Add(param.Id, param);
            return param;
        }

        public RGBAParameter CreateRGBAParameter()
        {
            var param = new RGBAParameter(FIdCounter++, this);
            FParams.Add(param.Id, param);
            return param;
        }

        public IGroupParameter CreateGroup()
        {
            var group = new GroupParameter(FIdCounter++, this);
            FParams.Add(group.Id, group);
            return group;
        }

        public void RemoveParameter(IParameter param)
        {
            FParams.Remove(param.Id);
            (param as Parameter.Parameter).Destroy();
        }

        public override void Update()
        {
            foreach (var param in FDirtyParams)
            {
                switch((param as RCP.Parameter.Parameter).Status)
                {
                    case Status.Update: SendToMultiple(Pack(RcpTypes.Command.Update, param)); break;
                    case Status.Remove: SendToMultiple(Pack(RcpTypes.Command.Remove, param)); break;
                }
            }

            base.Update();
        }
		
        public Action<Exception> OnError;


        public IParameter GetParameter(Int16 id)
		{
			return FParams[id];
		}

        public int ConnectionCount => FTransporters.Sum(t => t.ConnectionCount);
		
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
                        {
                            (packet.Data as Parameter.Parameter).CopyTo(FParams[packet.Data.Id]);
                            SendToMultiple(packet, senderId);
                        }
				        break;

                    case RcpTypes.Command.Updatevalue:
                        //TODO: actually only set the parameters value
                        if (FParams.ContainsKey(packet.Data.Id))
                            FParams.Remove(packet.Data.Id);
                        FParams.Add(packet.Data.Id, packet.Data);
                        SendToMultiple(packet, senderId);
                        break;

                    case RcpTypes.Command.Initialize:
				        //client requests all parameters
				        foreach (var param in FParams.Values)
                        {
                            (param as Parameter.Parameter).ResetForInitialize();
					        SendToOne(Pack(RcpTypes.Command.Update, param), senderId);
                        }
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