using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RCP.Model
{
    public abstract class Parameter: IParameter, IWriteable
    {
        public uint Id { get; private set; }
        public ITypeDefinition TypeDefinition { get; private set; }

        public string Label { get; set; }
        public string Description { get; set; }
        public int? Order { get; set; }

        public uint? Parent { get; set; }
        //public Widget Widget { get; set; }
        public byte[] Userdata { get; set; }

        public Parameter(uint id, ITypeDefinition typeDefinition)
        {
            Id = id;
            TypeDefinition = typeDefinition;
        }

        public void Write(BinaryWriter writer)
        {
            //mandatory
            writer.Write(Id, ByteOrder.BigEndian);
            TypeDefinition.Write(writer);

            //optional
            WriteValue(writer);

            if (!string.IsNullOrWhiteSpace(Label))
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Label);
                RcpTypes.TinyString.Write(Label, writer);
            }

            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Description);
                RcpTypes.ShortString.Write(Description, writer);
            }

            if (Order != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Order);
                writer.Write((int)Order, ByteOrder.BigEndian);
            }

            if (Parent != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Parent);
                writer.Write((uint)Parent, ByteOrder.BigEndian);
            }

            //widget
            //userdata

            //terminate
            writer.Write((byte)0);
        }

        protected abstract void WriteValue(BinaryWriter writer);

        public static Parameter Parse(KaitaiStream input)
        {
            // get mandatory id
            uint id = input.ReadU4be();

            var datatype = (RcpTypes.Datatype)input.ReadU1();
            if (!Enum.IsDefined(typeof(RcpTypes.Datatype), datatype))
                throw new RCPDataErrorException();

            Parameter parameter = null;

            switch (datatype)
            {
                case RcpTypes.Datatype.FixedArray:
                    {
                        dynamic arrayDefinition = ArrayDefinition<dynamic>.Parse(input);
                        parameter = (Parameter)ParameterFactory.CreateArrayParameter(id, arrayDefinition.Subtype.Datatype, arrayDefinition.Length);
                        break;
                    }

                default:
                    {
                        parameter = (Parameter)ParameterFactory.CreateParameter(id, datatype);
                        parameter.TypeDefinition.ParseOptions(input);
                        break;
                    }
            }

            parameter.ParseOptions(input);
            return parameter;
        }

        protected abstract bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option);

        private void ParseOptions(KaitaiStream input)
        {
            // get options from the stream
            while (true)
            {
                var code = input.ReadU1();
                if (code == 0)
                    break;

                var option = (RcpTypes.ParameterOptions)code;
                if (!Enum.IsDefined(typeof(RcpTypes.ParameterOptions), option))
                    throw new RCPDataErrorException();

                switch (option)
                {
                    case RcpTypes.ParameterOptions.Label:
                        Label = new RcpTypes.TinyString(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Description:
                        Description = new RcpTypes.ShortString(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Order:
                        Order = input.ReadS4be();
                        break;

                    case RcpTypes.ParameterOptions.Widget:
                        throw new RCPUnsupportedFeatureException();

                    case RcpTypes.ParameterOptions.Userdata:
                        Userdata = new RcpTypes.Userdata(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Value:
                    default:
                        if (!HandleOption(input, option))
                        {
                            throw new RCPDataErrorException();
                        }
                        break;
                }
            }
        }
    }

    public abstract class ValueParameter<T> : Parameter, IValueParameter<T>
    {
        public T Value { get; set; }

        public ValueParameter(uint id, IDefaultDefinition<T> typeDefinition): base (id, typeDefinition)
        { }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (Value != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                ((IDefaultDefinition<T>)TypeDefinition).WriteValue(writer, Value);
            }
        }
    }
}
