using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public class Parameter<T> : IParameter
    {
        public uint Id { get; set; }
        public dynamic TypeDefinition { get; set; }

        public T Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int? Order { get; set; }

        public uint? Parent { get; set; }
        public Widget Widget { get; set; }
        public object UserData { get; set; }

        public Parameter(uint id, dynamic typeDefinition)
        {
            Id = id;
            TypeDefinition = typeDefinition;
        }

        public static Parameter<T> Parse(KaitaiStream input)
        {
            // get mandatory id
            uint id = input.ReadU4be();
            // get mandatory type
            var typedefinition = TypeDefinition<T>.Parse(input);

            var parameter = new Parameter<T>(id, typedefinition);

            // get options from the stream
            while (true)
            {
                var code = input.ReadU1();
                if (code == 0) // terminator
                    break;

            	var property = (RcpTypes.Parameter)code;
				if (!Enum.IsDefined(typeof(RcpTypes.Parameter), property)) 
                	throw new RCPDataErrorException();
            	
            	if (typedefinition == null)
	            	MessageBox.Show(property.ToString() + " : ");
            	
                switch (property)
                {
                    case RcpTypes.Parameter.Value:
                      	switch ((RcpTypes.Datatype)typedefinition.Datatype) 
                		{
////                            case RcpTypes.Datatype.Boolean:
////                                ((RCPParameter<Boolean>)parameter).setValue(_io.readS1() > 0);
////                                break;
////                            case INT32:
////                                ((RCPParameter<Integer>)parameter).setValue(_io.readS4be());
////                                break;
                            case RcpTypes.Datatype.Float32:
                                ((dynamic)parameter).Value = input.ReadF4be();//(RCPParameter<Float>)parameter).setValue(_io.readF4be());
                                break;
////                            case TINY_STRING:
////                                break;
////                            case SHORT_STRING:
////                                break;
                            case RcpTypes.Datatype.String:
                				((dynamic)parameter).Value = new RcpTypes.LongString(input).Data;
                                break;
                        }

                        break;

                    case RcpTypes.Parameter.Label:
                        parameter.Label = new RcpTypes.TinyString(input).Data;
                        break;

                    case RcpTypes.Parameter.Description:
                        parameter.Description = new RcpTypes.ShortString(input).Data;
                        break;

                    case RcpTypes.Parameter.Order:
                        parameter.Order = input.ReadS4be();
                        break;

                    case RcpTypes.Parameter.Widget:
                        throw new RCPUnsupportedFeatureException();

                    case RcpTypes.Parameter.Userdata:
                        parameter.UserData = new RcpTypes.Userdata(input).Data;
                        break;

                    default:
                        break;
                }
            }

            return parameter;
        }

        public void Write(BinaryWriter writer)
        {
            //mandatory
            writer.Write(Id, ByteOrder.BigEndian);
            TypeDefinition.Write(writer);

            //optional
            if (Value != null)
        	{
        		writer.Write((byte)RcpTypes.Parameter.Value);
        		TypeDefinition.WriteValue(writer, Value);
        	}

            if (!string.IsNullOrWhiteSpace(Label))
            {
                writer.Write((byte)RcpTypes.Parameter.Label);
                RcpTypes.TinyString.Write(Label, writer);
            }

            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.Write((byte)RcpTypes.Parameter.Description);
                RcpTypes.ShortString.Write(Description, writer);
            }

            if (Order != null)
            {
                writer.Write((byte)RcpTypes.Parameter.Order);
                writer.Write((int)Order, ByteOrder.BigEndian);
            }

            if (Parent != null)
            {
                writer.Write((byte)RcpTypes.Parameter.Parent);
                writer.Write((uint)Parent, ByteOrder.BigEndian);
            }

            //widget
            //userdata

            //terminate
            writer.Write((byte)0);
        }
    }
}
