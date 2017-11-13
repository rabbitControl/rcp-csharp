using Kaitai;
using System;
using System.IO;

namespace RCP.Model
{
    public class Float32Definition : NumberDefinition<float>
    {
        public Float32Definition()
        : base(RcpTypes.Datatype.Float32)
        { }

        public override float ReadValue(KaitaiStream input)
        {
            return input.ReadF4be();
        }

        public override void WriteValue(BinaryWriter writer, float value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    	
        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var option = (RcpTypes.NumberOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.NumberOptions), option))
                throw new RCPDataErrorException();

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
            }

            return false;
        }
    }
}