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
            // get mandatory type
            var typedefinition = RCP.Model.TypeDefinition.Parse(input);
//			if (typedefinition != null)
//	        	MessageBox.Show(typedefinition.ToString() + " : ");
        	
            Parameter parameter = null;  

            // get options from the stream
            while (true)
            {
                var code = input.ReadU1();
                if (code == 0) // terminator
                    break;

                var option = (RcpTypes.ParameterOptions)code;
                if (!Enum.IsDefined(typeof(RcpTypes.ParameterOptions), option))
                    throw new RCPDataErrorException();

                switch (option)
                {
                    case RcpTypes.ParameterOptions.Value:
                        switch ((RcpTypes.Datatype)typedefinition.Datatype)
                        {
                            case RcpTypes.Datatype.Boolean:
                        	{
                                var param = new BooleanParameter(id, typedefinition as IBooleanDefinition);
                                param.Value = input.ReadByte() > 0;
                                parameter = param;
                                break;
                        	}
                        	
                        	case RcpTypes.Datatype.Int32:
                        	{
                                var param = new NumberParameter<int>(id, typedefinition as INumberDefinition<int>);
                                param.Value = input.ReadS4be();
                                parameter = param;
                                break;
                        	}
                        	
                        	case RcpTypes.Datatype.Float32:
                        	{
                                var param = new NumberParameter<float>(id, typedefinition as INumberDefinition<float>);
                                param.Value = input.ReadF4be();
                                parameter = param;
                                break;
                        	}
                            ////                            case TINY_STRING:
                            ////                                break;
                            ////                            case SHORT_STRING:
                            ////                                break;
                            case RcpTypes.Datatype.String:
                                var stringParameter = new StringParameter(id);
                                stringParameter.Value = new RcpTypes.LongString(input).Data;
                                parameter = stringParameter;
                                break;
                        }

                        break;

                    case RcpTypes.ParameterOptions.Label:
                        parameter.Label = new RcpTypes.TinyString(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Description:
                        parameter.Description = new RcpTypes.ShortString(input).Data;
                        break;

                    case RcpTypes.ParameterOptions.Order:
                        parameter.Order = input.ReadS4be();
                        break;

                    case RcpTypes.ParameterOptions.Widget:
                        throw new RCPUnsupportedFeatureException();

                    case RcpTypes.ParameterOptions.Userdata:
                        parameter.Userdata = new RcpTypes.Userdata(input).Data;
                        break;

                    default:
                        break;
                }
            }

            return parameter;
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
