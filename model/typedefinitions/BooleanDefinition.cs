using System;
using System.IO;
using Kaitai;

namespace RCP.Model
{
    public class BooleanDefinition : DefaultDefinition<bool>, IBooleanDefinition
    {
        public BooleanDefinition()
        : base(RcpTypes.Datatype.Boolean)
        {
        }
    	
    	public static new BooleanDefinition Parse(KaitaiStream input)
        {
            var boolDefinition = new BooleanDefinition();

            while (true)
            {
                var code = input.ReadU1();
                if (code == 0)
                    break;

                var option = (RcpTypes.BooleanOptions)code;
				if (!Enum.IsDefined(typeof(RcpTypes.BooleanOptions), option)) 
                	throw new RCPDataErrorException();

                switch (option)
                {
                    case RcpTypes.BooleanOptions.Default:
                        boolDefinition.Default = input.ReadU1() > 0;
                        break;
                }
            }

            return boolDefinition;
        }

        protected override void WriteProperties(BinaryWriter writer)
        {
            writer.Write((byte)RcpTypes.BooleanOptions.Default);
            writer.Write((byte)((bool)Default ? 1 : 0));
        }

        public override void WriteValue(BinaryWriter writer, bool value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}
