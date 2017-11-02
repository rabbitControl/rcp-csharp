using Kaitai;
using System;
using System.IO;

namespace RCP.Model
{
    public class Float32Definition : NumberDefinition<float>
    {
        public Float32Definition()
        : base(RcpTypes.Datatype.Float32) { }

    	public override void WriteValue(BinaryWriter writer, float value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    	
        public static new Float32Definition Parse(KaitaiStream input)
        {
            var definition = new Float32Definition();

            while (true)
            {
                var code = input.ReadU1();
                if (code == 0)
                    break;

                var option = (RcpTypes.NumberOptions)code;
				if (!Enum.IsDefined(typeof(RcpTypes.NumberOptions), option)) 
                	throw new RCPDataErrorException();

                switch (option)
                {
                    case RcpTypes.NumberOptions.Default:
                        definition.Default = input.ReadF4be();
                        break;
                    case RcpTypes.NumberOptions.Minimum:
                        definition.Minimum = input.ReadF4be();
                        break;
                    case RcpTypes.NumberOptions.Maximum:
                        definition.Maximum = input.ReadF4be();
                        break;
                    case RcpTypes.NumberOptions.Multipleof:
                        definition.MultipleOf = input.ReadF4be();
                        break;
                }
            }

            return definition;
        }
    }
}