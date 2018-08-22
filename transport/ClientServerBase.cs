using System;

using RCP.Protocol;
using System.Collections.Generic;
using System.Collections;
using RCP.Parameter;

namespace RCP
{
    public interface IParameterManager
    {
        void SetParameterDirty(IParameter param);
    }

    public abstract class ClientServerBase: IDisposable, IParameterManager
	{
        //public ILogger Logger { get; set; }
        protected Dictionary<Int16, IParameter> FParams = new Dictionary<Int16, IParameter>();
        protected HashSet<Int16> FDirtyParamIds = new HashSet<Int16>();

        protected IGroupParameter FRoot;
        public IGroupParameter Root => FRoot;

        public ClientServerBase()
        {
            FRoot = new GroupParameter(0, this);
        }

        protected Packet Pack(RcpTypes.Command command, IParameter parameter)
		{
			var packet = new Packet(command);
			packet.Data = parameter;
			
			return packet;
		}
		
		protected Packet Pack(RcpTypes.Command command, uint id)
		{
			var packet = new Packet(command);
			//packet.Data = new Parameter<T>(id, null);
			
			return packet;
		}
		
		protected Packet Pack(RcpTypes.Command command)
		{
			var packet = new Packet(command);
			
			return packet;
		}
		
		public virtual void Dispose()
		{
			//Logger = null;
		}

        public void SetParameterDirty(IParameter param)
        {
            if (!FDirtyParamIds.Contains(param.Id) && param.Id != 0)
                FDirtyParamIds.Add(param.Id);
        }

        public virtual void Update()
        {
            FDirtyParamIds.Clear();
        }

        public IParameter GetParameter(Int16 id)
        {
            if (FParams.ContainsKey(id))
                return FParams[id];
            else
                return null;
        }
    }
}