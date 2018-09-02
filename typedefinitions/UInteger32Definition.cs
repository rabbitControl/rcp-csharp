using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class UInteger32Definition : NumberDefinition<uint>
    {
        public UInteger32Definition()
        : base(RcpTypes.Datatype.Uint32)
        {
            FMinimum = uint.MinValue;
            FMaximum = uint.MaxValue;
            FMultipleOf = 1;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0;

            MinimumChanged = Minimum != uint.MinValue;
            MaximumChanged = Maximum != uint.MaxValue;
            MultipleOfChanged = MultipleOf != 1;
        }

        public override uint ReadValue(KaitaiStream input)
        {
            return input.ReadU4be();
        }

        public override void WriteValue(BinaryWriter writer, uint value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}