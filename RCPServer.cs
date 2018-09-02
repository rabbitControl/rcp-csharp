using System;
using System.IO;
using System.Collections.Generic;

using RCP.Protocol;
using Kaitai;
using System.Collections;
using System.Linq;
using RCP.Parameter;
using System.Numerics;
using System.Diagnostics;

namespace RCP
{
    public class RCPServer: ClientServerBase
    {
        List<IParameter> FParamsToRemove = new List<IParameter>();
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

        public IParameter CreateParameter(RcpTypes.Datatype type, string label = "", IGroupParameter group = null)
        {
            var param = TypeDefinition.Create(type).CreateParameter(FIdCounter++, this);
            param.Label = label;
            AddParameter(param, group);

            return param;
        }

        public IParameter CreateArrayParameter(RcpTypes.Datatype elementType, int[] structure, string label = "", IGroupParameter group = null)
        {
            var param = TypeDefinition.Create(elementType).CreateArray(structure).CreateParameter(FIdCounter++, this);
            param.Label = label;
            AddParameter(param);
            return param;
        }

        public IGroupParameter CreateGroup(string label = "", IGroupParameter group = null)
        {
            var param = TypeDefinition.Create(RcpTypes.Datatype.Group).CreateParameter(FIdCounter++, this) as IGroupParameter;
            param.Label = label;
            AddParameter(param, group);
            return param;
        }

        public void AddParameter(IParameter param, IGroupParameter group = null)
        {
            if (!FParams.ContainsKey(param.Id))
                FParams.Add(param.Id, param);

            if (group == null)
                group = Root;

            (group as GroupParameter).AddParameter(param);
        }

        public void RemoveParameter(IParameter param)
        {
            FParams.Remove(param.Id);
            FParamsToRemove.Add(param);
        }

        public override void Update()
        {
            foreach (var param in FParamsToRemove)
                SendToMultiple(Pack(RcpTypes.Command.Remove, param));
            FParamsToRemove.Clear();

            foreach (var id in FDirtyParamIds)
                SendToMultiple(Pack(RcpTypes.Command.Update, FParams[id]));

            base.Update();
        }
		
        public Action<Exception> OnError;

        public Action<string> Log;

        
		
		//public IEnumerable<IParameter> GetParametersByParent(Int16 id)
		//{
		//	return FParams.Values.Where(p => p.Parent.HasValue ? p.Parent.Value == id : false);
		//}

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
			    var packet = Packet.Parse(new KaitaiStream(bytes), null);
				//MessageBox.Show(packet.Command.ToString());
		        switch (packet.Command)
		        {
			        case RcpTypes.Command.Update:
                        Log?.Invoke("received update");
                        if (FParams.ContainsKey(packet.Data.Id))
                        {
                            (FParams[packet.Data.Id] as Parameter.Parameter).CopyFrom(packet.Data);
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
                        Log?.Invoke("received init");
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
                Trace.TraceError(e.Message);
            }
        }
		
		void SendToMultiple(Packet packet, object exceptClientId = null)
		{
			using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                Log?.Invoke("sending to multiple");
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
                Log?.Invoke("sending to one");
                packet.Write(writer);
                var bytes = stream.ToArray();
                foreach (var transporter in FTransporters)
                    transporter.SendToOne(bytes, clientId);
            }
		}
        #endregion
    
    }
}