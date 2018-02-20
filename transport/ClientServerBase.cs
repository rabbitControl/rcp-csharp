using System;

using RCP.Protocol;
using System.Collections.Generic;
using System.Collections;

namespace RCP
{
    public abstract class ClientServerBase: IDisposable
	{
		//public ILogger Logger { get; set; }
		
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
	}

    public class StructuralEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }

        private static StructuralEqualityComparer<T> defaultComparer;
        public static StructuralEqualityComparer<T> Default
        {
            get
            {
                StructuralEqualityComparer<T> comparer = defaultComparer;
                if (comparer == null)
                {
                    comparer = new StructuralEqualityComparer<T>();
                    defaultComparer = comparer;
                }
                return comparer;
            }
        }
    }
}