using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Integer64Definition : NumberDefinition<long>
    {
        public Integer64Definition()
        : base(RcpTypes.Datatype.Int64)
        {
            FMinimum = long.MinValue;
            FMaximum = long.MaxValue;
            FMultipleOf = 1;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0;

            MinimumChanged = Minimum != long.MinValue;
            MaximumChanged = Maximum != long.MaxValue;
            MultipleOfChanged = MultipleOf != 1;
        }

        public override long ReadValue(KaitaiStream input)
        {
            return input.ReadS8be();
        }

        public override void WriteValue(BinaryWriter writer, long value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}