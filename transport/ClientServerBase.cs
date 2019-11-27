using System;

using RCP.Protocol;
using System.Collections.Generic;
using RCP.Parameters;
using RCP.Types;
using System.Threading;
using System.ComponentModel;

namespace RCP
{
    public interface IParameterManager
    {
        Parameter GetParameter(Int16 id);
    }

    public abstract class ClientServerBase : IDisposable, IParameterManager
	{
        protected const string RCP_PROTOCOL_VERSION = "0.0.1";
        private readonly SynchronizationContext FContext;
        protected Dictionary<Int16, Parameter> FParams = new Dictionary<Int16, Parameter>();
        bool FIsDirty;

        public GroupParameter Root { get; }
        public bool AutoUpdate { get; set; } = true;

        public ClientServerBase()
        {
            FContext = SynchronizationContext.Current;
            Root = new GroupParameter(0, this, new GroupDefinition());
        }

        public event EventHandler<IParameter> ParameterAdded;
        public event EventHandler<IParameter> ParameterRemoved;

        protected Packet Pack(RcpTypes.Command command, Parameter parameter)
		{
			var packet = new Packet(command);
			packet.Data = parameter;
			
			return packet;
		}

        protected Packet Pack(RcpTypes.Command command, short id)
        {
            var packet = new Packet(command);
            packet.Data = id;

            return packet;
        }

        protected Packet Pack(RcpTypes.Command command, InfoData infoData)
        {
            var packet = new Packet(command);
            packet.Data = infoData;

            return packet;
        }

        protected Packet Pack(RcpTypes.Command command)
		{
			var packet = new Packet(command);
			
			return packet;
		}

        public abstract void Dispose();
        public abstract void Update();

        public Parameter GetParameter(Int16 id)
        {
            if (FParams.TryGetValue(id, out Parameter parameter))
                return parameter;
            else
                return null;
        }

        public virtual void AddParameter(Parameter parameter)
        {
            var id = parameter.Id;
            if (!FParams.ContainsKey(id))
            {
                FParams.Add(id, parameter);
                OnParameterAdded(parameter);
            }
        }

        public virtual void RemoveParameter(Parameter parameter)
        {
            if (FParams.Remove(parameter.Id))
                OnParameterRemoved(parameter);
        }

        protected virtual void OnParameterAdded(Parameter parameter)
        {
            parameter.PropertyChanged += HandleParameterUpdated;
            ParameterAdded?.Invoke(this, parameter);
        }

        protected virtual void OnParameterRemoved(Parameter parameter)
        {
            parameter.PropertyChanged -= HandleParameterUpdated;
            ParameterRemoved?.Invoke(this, parameter);
        }

        void HandleParameterUpdated(object sender, PropertyChangedEventArgs args)
        {
            if (!AutoUpdate)
                return;
            if (FIsDirty)
                return;
            FIsDirty = true;
            FContext.Post(_ =>
            {
                try
                {
                    Update();
                }
                finally
                {
                    FIsDirty = false;
                }
            }, null);
        }
    }
}