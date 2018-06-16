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

        public INumberParameter<T> CreateNumberParameter<T>(string label = "", IGroupParameter group = null) where T: struct
        {
            IParameter param = null;
            //if (typeof(T) == typeof(float))
            //    param = new Float32Parameter(FIdCounter++, this);
            //else if (typeof(T) == typeof(int))
            //    param = new Integer32Parameter(FIdCounter++, this);
            //else if (typeof(T) == typeof(Vector2))
            //    param = new Vector2f32Parameter(FIdCounter++, this);
            //else if (typeof(T) == typeof(Vector3))
            //    param = new Vector3f32Parameter(FIdCounter++, this);

            param = new NumberParameter<T>(FIdCounter++, this);

            param.Label = label;
            AddParameter(param, group);

            return param as NumberParameter<T>;
        }

        //public IArrayParameter<T> CreateArrayNumberParameter<T>(string label = "", IGroupParameter group = null) where T : struct
        //{
        //    var param = new ArrayParameter<T>(FIdCounter++, this);
        //    param.Label = label;
        //    AddParameter(param);
        //    return param;
        //}

        //public IStringParameter CreateStringParameter(string label = "", IGroupParameter group = null)
        //{
        //    var param = new StringParameter(FIdCounter++, this);
        //    param.Label = label;
        //    AddParameter(param, group);
        //    return param;
        //}

        //public IStringArrayParameter<T> CreateStringArrayParameter<T>(string label, params int[] structure)
        //{
        //    var param = new StringArrayParameter<T>(FIdCounter++, this, structure);
        //    param.Label = label;
        //    AddParameter(param);
        //    return param;
        //}

        //public IEnumParameter CreateEnumParameter(string label = "", IGroupParameter group = null)
        //{
        //    var param = new EnumParameter(FIdCounter++, this);
        //    param.Label = label;
        //    AddParameter(param, group);
        //    return param;
        //}

        //public IRGBAParameter CreateRGBAParameter(string label = "", IGroupParameter group = null)
        //{
        //    var param = new RGBAParameter(FIdCounter++, this);
        //    param.Label = label;
        //    AddParameter(param, group);
        //    return param;
        //}

        //public IGroupParameter CreateGroup(string label = "", IGroupParameter group = null)
        //{
        //    var param = new GroupParameter(FIdCounter++, this);
        //    param.Label = label;
        //    AddParameter(param, group);
        //    return param;
        //}

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

            foreach (var param in FDirtyParams)
                SendToMultiple(Pack(RcpTypes.Command.Update, param));

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
			    var packet = Packet.Parse(new KaitaiStream(bytes), this);
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

        //public void WriteValue(this float value, BinaryWriter writer)
        //{
        //    writer.Write(value, ByteOrder.BigEndian);
        //}

        //public float ReadValue(this float , KaitaiStream input)
        //{
        //    return input.ReadF4be();
        //}        
    }
}