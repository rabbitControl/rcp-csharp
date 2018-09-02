using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Float64Definition : NumberDefinition<double>
    {
        public Float64Definition()
        : base(RcpTypes.Datatype.Float64)
        {
            FMinimum = double.MinValue;
            FMaximum = double.MaxValue;
            FMultipleOf = 0.01f;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0.0f;

            MinimumChanged = Minimum != double.MinValue;
            MaximumChanged = Maximum != double.MaxValue;
            MultipleOfChanged = MultipleOf != 0.01f;
        }

        public override double ReadValue(KaitaiStream input)
        {
            return input.ReadF8be();
        }

        public override void WriteValue(BinaryWriter writer, double value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}