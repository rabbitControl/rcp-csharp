using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public abstract class RCPNumber<T> : TypeDefinition<T>
    {
        public T Minimum { get; set; }
        public T Maximum { get; set; }
        public T MultipleOf { get; set; }
        public RcpTypes.NumberScale? Scale { get; set; }
        public string Unit { get; set; }

        public RCPNumber(RcpTypes.Datatype datatype)
        : base(datatype) { }

        public static void Parse(RCPNumber<T> number, RcpTypes.NumberProperty property, KaitaiStream input)
        {
            switch (property)
            {
                case RcpTypes.NumberProperty.Scale:
                    number.Scale = (RcpTypes.NumberScale)input.ReadU1();
                    break;

                case RcpTypes.NumberProperty.Unit:
                    number.Unit = new RcpTypes.TinyString(input).Data;
                    break;

                    //            	default:
                    //                	// not a number data id!!
                    //                	throw new RCPDataErrorException();
            }
        }

    	protected abstract T TypesDefault();

        protected override void WriteProperties(BinaryWriter writer)
        {
            if (!Default.Equals(TypesDefault()))
            {
                writer.Write((byte)RcpTypes.NumberProperty.Default);
                WriteValue(writer, Default);
            }

            if (!Minimum.Equals(TypesDefault()))
            {
                writer.Write((byte)RcpTypes.NumberProperty.Minimum);
                WriteValue(writer, Minimum);
            }

            if (!Maximum.Equals(TypesDefault()))
            {
                writer.Write((byte)RcpTypes.NumberProperty.Maximum);
                WriteValue(writer, Maximum);
            }

            if (!MultipleOf.Equals(TypesDefault()))
            {
                writer.Write((byte)RcpTypes.NumberProperty.Multipleof);
                WriteValue(writer, MultipleOf);
            }

            if (Scale != null)
            {
                writer.Write((byte)RcpTypes.NumberProperty.Scale);
                writer.Write((byte)Scale);
            }
        	
        	if (!string.IsNullOrWhiteSpace(Unit))
            {
                writer.Write((byte)RcpTypes.NumberProperty.Unit);
                writer.Write(Unit);
            }
        }
    }
}