using Kaitai;
using RCP.Parameters;
using RCP.Protocol;
using RCP.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace RCP
{
    public class RCPServer: ClientServerBase
    {
        List<short> FParamsToRemove = new List<short>();
		List<IServerTransporter> FTransporters = new List<IServerTransporter>();
        Int16 FIdCounter = 1;

        public IReadOnlyDictionary<Int16, Parameter> Parameters => FParams;

        public string ApplicationId { get; }

        public RCPServer(string applicationId = "")
            : base()
        {
            ApplicationId = applicationId;
        }
 
        public RCPServer(IServerTransporter transporter, string applicationId = "")
            : this()
        {
            AddTransporter(transporter);

            ApplicationId = applicationId;
        }

		public override void Dispose()
		{
            foreach (var transporter in FTransporters)
            {
                transporter.Received = null;
                transporter.Dispose();
            }
			
			FTransporters.Clear();
		}

        public Parameter CreateParameter(RcpTypes.Datatype type, string label = "", GroupParameter group = null)
        {
            var param = Parameter.Create(this, FIdCounter++, type);
            return AddAndReturn(param, label, group);
        }

        public ArrayParameter<T> CreateArrayParameter<T>(string label = "", params int[] structure) => CreateArrayParameter<T>(label, null, structure);

        public ArrayParameter<T> CreateArrayParameter<T>(string label = "", GroupParameter group = null, int[] structure = null)
        {
            var elementType = TypeDefinition.GetDatatype(typeof(T));
            var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Array, elementType, structure) as ArrayParameter<T>;
            return AddAndReturn(param, label, group);
        }

        public Parameter CreateBangParameter(string label = "", GroupParameter group = null)
        {
            var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Bang);
            return AddAndReturn(param, label, group);
        }

        public Parameter CreateRangeParameter(RcpTypes.Datatype elementType, string label = "", GroupParameter group = null)
        {
            var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Range, elementType);
            return AddAndReturn(param, label, group);
        }

        public NumberParameter<T> CreateNumberParameter<T>(string label = "", GroupParameter group = null) /*where T : struct*/
        {
            var datatype = TypeDefinition.GetDatatype(typeof(T));
            var param = (NumberParameter<T>)CreateParameter(datatype, label, group);
            return AddAndReturn(param, label, group);
        }

        public ValueParameter<T> CreateValueParameter<T>(string label = "", GroupParameter group = null)
        {
            var datatype = TypeDefinition.GetDatatype(typeof(T));
            var param = CreateParameter(datatype, label, group) as ValueParameter<T>;
            return AddAndReturn(param, label, group);
        }

        public StringParameter CreateStringParameter(string label = "", GroupParameter group = null)
        {
            var param = CreateParameter(RcpTypes.Datatype.String, label, group) as StringParameter;
            return AddAndReturn(param, label, group);
        }

        public UriParameter CreateUriParameter(string label = "", GroupParameter group = null)
        {
            var param = CreateParameter(RcpTypes.Datatype.Uri, label, group) as UriParameter;
            return AddAndReturn(param, label, group);
        }

        public ImageParameter CreateImageParameter(string label = "", GroupParameter group = null)
        {
            var param = CreateParameter(RcpTypes.Datatype.Image, label, group) as ImageParameter;
            return AddAndReturn(param, label, group);
        }

        public ArrayParameter<string> CreateUriArrayParameter(string label, params int[] structure)
        {
            var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Array, RcpTypes.Datatype.Uri, structure) as ArrayParameter<string>;
            param.Label = label;
            AddParameter(param);
            return param;
        }

        public EnumParameter CreateEnumParameter(string label = "", GroupParameter group = null)
        {
            var param = CreateParameter(RcpTypes.Datatype.Enum, label, group) as EnumParameter;
            return AddAndReturn(param, label, group);
        }

        public ArrayParameter<string> CreateEnumArrayParameter(string label, params int[] structure)
        {
            var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Array, RcpTypes.Datatype.Enum, structure) as ArrayParameter<string>;
            param.Label = label;
            AddParameter(param);
            return param;
        }

        public GroupParameter CreateGroup(string label = "", GroupParameter group = null)
        {
            var param = Parameter.Create(this, FIdCounter++, RcpTypes.Datatype.Group) as GroupParameter;
            return AddAndReturn(param, label, group);
        }

        TParameter AddAndReturn<TParameter>(TParameter param, string label, GroupParameter group) where TParameter : Parameter
        {
            param.Label = label;
            AddParameter(param, group);
            return param;
        }

        public void AddParameter(Parameter param, GroupParameter group)
        {
            base.AddParameter(param);

            if (group == null)
                group = Root;

            group.AddParameter(param);
        }

        public override void RemoveParameter(Parameter param)
        {
            FParams.Remove(param.Id);
            FParamsToRemove.Add(param.Id);
        }

        public override void Update()
        {
            foreach (var id in FParamsToRemove)
                SendToMultiple(Pack(RcpTypes.Command.Remove, id));
            FParamsToRemove.Clear();

            foreach (var parameter in FParams.Values)
                if (parameter.OnlyValueChanged)
                    SendToMultiple(Pack(RcpTypes.Command.Updatevalue, parameter));
                else if (parameter.IsDirty)
                    SendToMultiple(Pack(RcpTypes.Command.Update, parameter));
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
			    var packet = Packet.Parse(new KaitaiStream(bytes), this);
				//MessageBox.Show(packet.Command.ToString());
		        switch (packet.Command)
		        {
                    case RcpTypes.Command.Info:
                        {
                            if (packet.Data == null)
                            {
                                Log?.Invoke("received: version request");
                                //answer with a version
                                SendToOne(Pack(RcpTypes.Command.Info, new InfoData(RCP_PROTOCOL_VERSION, ApplicationId)), senderId);
                            }
                            else
                            {
                                var info = (packet.Data as InfoData);
                                //set version status
                            }
                            break;
                        }

                    case RcpTypes.Command.Update:
                        {
                            Log?.Invoke("received: update");
                            var param = packet.Data as Parameter;
                            if (FParams.ContainsKey(param.Id))
                                SendToMultiple(bytes, senderId);
                            param.RaiseEvents();
                            break;
                        }

                    case RcpTypes.Command.Updatevalue:
                        {
                            //TODO: actually only set the parameters value
                            Log?.Invoke("received: update value");
                            var param = packet.Data as Parameter;
                            if (FParams.ContainsKey(param.Id))
                                SendToMultiple(bytes, senderId);
                            param.RaiseEvents();
                            break;
                        }

                    case RcpTypes.Command.Initialize:
                        {
                            Log?.Invoke("received: init");
				            //client requests all parameters
				            foreach (var param in FParams.Values)
                            {
                                param.ResetForInitialize();
					            SendToOne(Pack(RcpTypes.Command.Update, param), senderId);
                            }
				            break;
                        }
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

        void SendToMultiple(byte[] bytes, object exceptClientId = null)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                Log?.Invoke("sending to multiple");
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