using Kaitai;
using System.IO;

namespace RCP.Types
{
    public sealed class UInteger64Definition : NumberDefinition<ulong>
    {
        protected override ulong DefaultMinimum => ulong.MinValue;
        protected override ulong DefaultMaximum => ulong.MaxValue;
        protected override ulong DefaultMulitpleOf => 1;

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