using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public class RCPFloat32 : RCPNumber<float>
    {
        public RCPFloat32()
        : base(RcpTypes.Datatype.Float32) { }

    	protected override float TypesDefault()
    	{
    		return 0f;
    	}
    	
    	public override void WriteValue(BinaryWriter writer, float value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    	
        public static new RCPFloat32 Parse(KaitaiStream input)
        {
            var floatDefinition = new RCPFloat32();

            while (true)
            {
                var code = input.ReadU1();
                if (code == 0)
                    break;

                var property = (RcpTypes.NumberOptions)code;
				if (!Enum.IsDefined(typeof(RcpTypes.NumberOptions), property)) 
                	throw new RCPDataErrorException();

                switch (property)
                {
                    case RcpTypes.NumberOptions.Default:
                        floatDefinition.Default = input.ReadF4be();
                        break;
                    case RcpTypes.NumberOptions.Minimum:
                        floatDefinition.Minimum = input.ReadF4be();
                        break;
                    case RcpTypes.NumberOptions.Maximum:
                        floatDefinition.Maximum = input.ReadF4be();
                        break;
                    case RcpTypes.NumberOptions.Multipleof:
                        floatDefinition.MultipleOf = input.ReadF4be();
                        break;
                }
            }

            return floatDefinition;
        }
    }
}