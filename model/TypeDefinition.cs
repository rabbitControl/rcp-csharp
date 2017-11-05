using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RCP.Model
{
    public abstract class TypeDefinition : ITypeDefinition
    {
        public RcpTypes.Datatype Datatype { get; private set; }

        public TypeDefinition(RcpTypes.Datatype datatype)
        {
            Datatype = datatype;
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

        public static TypeDefinition Parse(KaitaiStream input)
        {
            var code = input.ReadByte();

            var datatype = (RcpTypes.Datatype)code;
            if (!Enum.IsDefined(typeof(RcpTypes.Datatype), datatype))
                throw new RCPDataErrorException();

//        	if (datatype != null)
//	            MessageBox.Show(datatype.ToString() + " : ");

            TypeDefinition definition = null;
            switch (datatype)
            {
                case RcpTypes.Datatype.Boolean: definition = BooleanDefinition.Parse(input); break;
                case RcpTypes.Datatype.Int32: definition = Integer32Definition.Parse(input); break;
                //				case RcpTypes.Datatype.Uint64: definition = RCPUInt64.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Int64: definition = RCPInt64.Parse(input) as TypeDefinition<T>; break;
                case RcpTypes.Datatype.Float32: definition = Float32Definition.Parse(input); break;
                //				case RcpTypes.Datatype.Float64: definition = RCPFloat64.Parse(input) as TypeDefinition<T>; break;
                case RcpTypes.Datatype.String: definition = StringDefinition.Parse(input); break;
            }
        	
            return definition;
        }
    }

    public abstract class DefaultDefinition<T>: TypeDefinition, IDefaultDefinition<T>
    {
        public T Default { get; set; }

        public DefaultDefinition(RcpTypes.Datatype datatype) : base(datatype)
        { }

        protected override void WriteProperties(BinaryWriter writer)
        {
            if (Default != null)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Default);
                WriteValue(writer, Default);
            }
        }

        public abstract void WriteValue(BinaryWriter writer, T value);
    }
}