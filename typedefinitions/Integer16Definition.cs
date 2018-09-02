using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Integer16Definition : NumberDefinition<short>
    {
        public Integer16Definition()
        : base(RcpTypes.Datatype.Int16)
        {
            FMinimum = short.MinValue;
            FMaximum = short.MaxValue;
            FMultipleOf = 1;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0;

            MinimumChanged = Minimum != short.MinValue;
            MaximumChanged = Maximum != short.MaxValue;
            MultipleOfChanged = MultipleOf != 1;
        }

        public override short ReadValue(KaitaiStream input)
        {
            return input.ReadS2be();
        }

        public override void WriteValue(BinaryWriter writer, short value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}