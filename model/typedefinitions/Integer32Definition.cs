using Kaitai;
using System;
using System.IO;

namespace RCP.Model
{
    public class Integer32Definition : NumberDefinition<int>
    {
        public Integer32Definition()
        : base(RcpTypes.Datatype.Int32) { }

    	public override void WriteValue(BinaryWriter writer, int value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    	
        public static new Integer32Definition Parse(KaitaiStream input)
        {
            var definition = new Integer32Definition();

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
                        definition.Default = input.ReadS4be();
                        break;
                    case RcpTypes.NumberOptions.Minimum:
                        definition.Minimum = input.ReadS4be();
                        break;
                    case RcpTypes.NumberOptions.Maximum:
                        definition.Maximum = input.ReadS4be();
                        break;
                    case RcpTypes.NumberOptions.Multipleof:
                        definition.MultipleOf = input.ReadS4be();
                        break;
                }
            }

            return definition;
        }
    }
}