using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public abstract class TypeDefinition<T>
    {
        public RcpTypes.Datatype Datatype { get; set; }
        public T Default { get; set; }

        public TypeDefinition(RcpTypes.Datatype datatype)
        {
            Datatype = datatype;
        }

        public static dynamic Parse(KaitaiStream input)
        {
            var code = input.ReadByte();
        	
        	var datatype = (RcpTypes.Datatype)code;
			if (!Enum.IsDefined(typeof(RcpTypes.Datatype), datatype)) 
            	throw new RCPDataErrorException();

//        	if (datatype != null)
//	            	MessageBox.Show(datatype.ToString() + " : ");
        	
            dynamic definition = null;
            switch (datatype)
            {
                //case RcpTypes.Datatype.Boolean: definition = RCPBoolean.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Uint8: definition = RCPUInt8.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Int8: definition = RCPFInt8.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Uint16: definition = RCPUInt16.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Int16: definition = RCPInt16.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Uint32: definition = RCPUInt32.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Int32: definition = RCPInt32.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Uint64: definition = RCPUInt64.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Int64: definition = RCPInt64.Parse(input) as TypeDefinition<T>; break;
                case RcpTypes.Datatype.Float32: definition = RCPFloat32.Parse(input); break;
                //				case RcpTypes.Datatype.Float64: definition = RCPFloat64.Parse(input) as TypeDefinition<T>; break;
                case RcpTypes.Datatype.String: definition = RCPString.Parse(input); break;
            }

            return definition;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)Datatype);

            //write type specific stuff
            WriteProperties(writer);

            //terminate
            writer.Write((byte)0);
        }

        protected abstract void WriteProperties(BinaryWriter writer);
        public abstract void WriteValue(BinaryWriter writer, T value);
    }
}