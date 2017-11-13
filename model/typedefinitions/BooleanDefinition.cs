using System;
using System.IO;
using Kaitai;

namespace RCP.Model
{
    public class BooleanDefinition : DefaultDefinition<bool>, IBooleanDefinition
    {
        public BooleanDefinition()
        : base(RcpTypes.Datatype.Boolean)
        { }

        public override bool ReadValue(KaitaiStream input)
        {
            return input.ReadU1() > 0;
        }

        public override void WriteValue(BinaryWriter writer, bool value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    	
        protected override void WriteOptions(BinaryWriter writer)
        {
            writer.Write((byte)RcpTypes.BooleanOptions.Default);
            writer.Write((byte)((bool)Default ? 1 : 0));
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.BooleanOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.BooleanOptions), option))
                throw new RCPDataErrorException();

            switch (option)
            {
                case RcpTypes.BooleanOptions.Default:
                    Default = ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
