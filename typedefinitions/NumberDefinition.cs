using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public abstract class NumberDefinition<T> : DefaultDefinition<T>, INumberDefinition<T> where T: struct
    {
        public Nullable<T> Minimum { get; set; }
        public Nullable<T> Maximum { get; set; }
        public Nullable<T> MultipleOf { get; set; }
        public RcpTypes.NumberScale? Scale { get; set; }
        public string Unit { get; set; }

        public NumberDefinition(RcpTypes.Datatype datatype)
        : base(datatype)
        { }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (Minimum != null)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Minimum);
                WriteValue(writer, (T)Minimum);
            }

            if (Maximum != null)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Maximum);
                WriteValue(writer, (T)Maximum);
            }

            if (MultipleOf != null)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Multipleof);
                WriteValue(writer, (T)MultipleOf);
            }

            if (Scale != null)
            {
                writer.Write((byte)RcpTypes.NumberOptions.Scale);
                writer.Write((byte)Scale);
            }
        	
        	if (!string.IsNullOrWhiteSpace(Unit))
            {
                writer.Write((byte)RcpTypes.NumberOptions.Unit);
                writer.Write(Unit);
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var result = base.HandleOption(input, code);
            if (result)
                return result;

            var option = (RcpTypes.NumberOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.NumberOptions), option))
                throw new RCPDataErrorException("NumberDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.NumberOptions.Default:
                    Default = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Minimum:
                    Minimum = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Maximum:
                    Maximum = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Multipleof:
                    MultipleOf = ReadValue(input);
                    return true;

                case RcpTypes.NumberOptions.Scale:
                    Scale = (RcpTypes.NumberScale)input.ReadU1();
                    return true;

                case RcpTypes.NumberOptions.Unit:
                    Unit = new RcpTypes.TinyString(input).Data;
                    return true;

                    //            	default:
                    //                	// not a number data id!!
                    //                	throw new RCPDataErrorException();
            }

            return false;
        }
    }
}