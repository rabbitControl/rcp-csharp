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

        public virtual void Write(BinaryWriter writer)
        {
            writer.Write((byte)Datatype);

            //write type specific stuff
            WriteOptions(writer);

            //terminate
            writer.Write((byte)0);
        }

        protected abstract void WriteOptions(BinaryWriter writer);

        public void ParseOptions(KaitaiStream input)
        {
            while (true)
            {
                var code = input.ReadU1();
                if (code == 0) // terminator
                    break;

                // handle option in specific implementation
                if (!HandleOption(input, code))
                {
                    throw new RCPDataErrorException();
                }
            }
        }

        protected abstract bool HandleOption(KaitaiStream input, byte option);
    }

    public abstract class DefaultDefinition<T>: TypeDefinition, IDefaultDefinition<T>
    {
        public T Default { get; set; }

        public DefaultDefinition(RcpTypes.Datatype datatype) : base(datatype)
        { }

        public abstract T ReadValue(KaitaiStream input);
        public abstract void WriteValue(BinaryWriter writer, T value);

        protected override void WriteOptions(BinaryWriter writer)
        {
            if (Default != null)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Default);
                WriteValue(writer, Default);
            }
        }

        public static ITypeDefinition Parse(KaitaiStream input)
        {
            var code = input.ReadByte();

            var datatype = (RcpTypes.Datatype)code;
            if (!Enum.IsDefined(typeof(RcpTypes.Datatype), datatype))
                throw new RCPDataErrorException();

            //        	if (datatype != null)
            //	            MessageBox.Show(datatype.ToString() + " : ");
            ITypeDefinition definition = null;
            switch (datatype)
            {
                case RcpTypes.Datatype.Boolean: definition = new BooleanDefinition(); break;
                case RcpTypes.Datatype.Int32: definition = new Integer32Definition(); break;
                //				case RcpTypes.Datatype.Uint64: definition = RCPUInt64.Parse(input) as TypeDefinition<T>; break;
                //				case RcpTypes.Datatype.Int64: definition = RCPInt64.Parse(input) as TypeDefinition<T>; break;
                case RcpTypes.Datatype.Float32: definition = new Float32Definition(); break;
                //				case RcpTypes.Datatype.Float64: definition = RCPFloat64.Parse(input) as TypeDefinition<T>; break;
                case RcpTypes.Datatype.String: definition = new StringDefinition(); break;
                case RcpTypes.Datatype.Rgba: definition = new RGBADefinition(); break;
            }

            if (definition != null)
            {
                definition.ParseOptions(input);
                return definition;
            }

            throw new RCPDataErrorException();
        }
    }
}