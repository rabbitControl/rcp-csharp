using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public sealed class UInteger64Definition : NumberDefinition<ulong>
    {
        public UInteger64Definition()
        : base(RcpTypes.Datatype.Uint64)
        {
            FMinimum = ulong.MinValue;
            FMaximum = ulong.MaxValue;
            FMultipleOf = 1;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0;

            MinimumChanged = Minimum != ulong.MinValue;
            MaximumChanged = Maximum != ulong.MaxValue;
            MultipleOfChanged = MultipleOf != 1;
        }

        public override ulong ReadValue(KaitaiStream input)
        {
            return input.ReadU8be();
        }

        public override void WriteValue(BinaryWriter writer, ulong value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}