using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class UInteger8Definition : NumberDefinition<byte>
    {
        public UInteger8Definition()
        : base(RcpTypes.Datatype.Uint8)
        {
            FMinimum = byte.MinValue;
            FMaximum = byte.MaxValue;
            FMultipleOf = 1;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0;

            MinimumChanged = Minimum != byte.MinValue;
            MaximumChanged = Maximum != byte.MaxValue;
            MultipleOfChanged = MultipleOf != 1;
        }

        public override byte ReadValue(KaitaiStream input)
        {
            return input.ReadU1();
        }

        public override void WriteValue(BinaryWriter writer, byte value)
        {
            writer.Write(value);
        }
    }
}