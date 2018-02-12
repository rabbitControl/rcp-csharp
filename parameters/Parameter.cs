using System;
using System.IO;
using Kaitai;

using RCP.Exceptions;
using RCP.Protocol;

namespace RCP.Parameter
{
    public abstract class Parameter: IParameter, IWriteable
    {
        public int Id { get; private set; }
        public ITypeDefinition TypeDefinition { get; private set; }

        public string Label { get; set; }
        public string Description { get; set; }
        public int? Order { get; set; }

        public int? Parent { get; set; }
        //public Widget Widget { get; set; }
        public byte[] Userdata { get; set; }

        public Parameter(int id, ITypeDefinition typeDefinition)
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
        	
            if (Userdata != null)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Userdata);
            	writer.Write(Userdata.Length, ByteOrder.BigEndian);
                writer.Write(Userdata);
            }

            //terminate
            writer.Write((byte)0);
        }

        protected abstract void WriteValue(BinaryWriter writer);

        public static Parameter Parse(KaitaiStream input)
        {
            // get mandatory id
            int id = input.ReadS4be();

            var datatype = (RcpTypes.Datatype)input.ReadU1();
            if (!Enum.IsDefined(typeof(RcpTypes.Datatype), datatype))
                throw new RCPDataErrorException("Parameter parsing: Unknown datatype!");

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

        protected virtual bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            return false;
        }

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
                    throw new RCPDataErrorException("Parameter parsing: Unknown option: " + option.ToString());

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
                            throw new RCPUnsupportedFeatureException();
                        }
                        break;
                }
            }
        }
    }

    public abstract class ValueParameter<T> : Parameter, IValueParameter<T>
    {
        public T Value { get; set; }

        public ValueParameter(int id, IDefaultDefinition<T> typeDefinition): base (id, typeDefinition)
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