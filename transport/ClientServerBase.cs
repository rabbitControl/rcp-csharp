using System;

using RCP.Protocol;
using System.Collections.Generic;
using RCP.Parameters;
using RCP.Types;

namespace RCP
{
    public interface IParameterManager
    {
        Parameter GetParameter(Int16 id);
    }

    public abstract class ClientServerBase: IDisposable, IParameterManager
	{
        protected Dictionary<Int16, Parameter> FParams = new Dictionary<Int16, Parameter>();

        public GroupParameter Root { get; }

        public ClientServerBase()
        {
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
                FParams.Add(id, parameter);
            ParameterAdded?.Invoke(this, parameter);
        }

        public virtual void RemoveParameter(Parameter parameter)
        {
            var id = parameter.Id;
            FParams.Remove(id);
            ParameterRemoved?.Invoke(this, parameter);
        }
    }
}