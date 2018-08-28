using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Float32Definition : NumberDefinition<float>
    {
        public Float32Definition()
        : base(RcpTypes.Datatype.Float32)
        {
            FMinimum = float.MinValue;
            FMaximum = float.MaxValue;
            FMultipleOf = 0.01f;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0.0f;

            MinimumChanged = Minimum != float.MinValue;
            MaximumChanged = Maximum != float.MaxValue;
            MultipleOfChanged = MultipleOf != 0.01f;
        }

        public override float ReadValue(KaitaiStream input)
        {
            return input.ReadF4be();
        }

        public override void WriteValue(BinaryWriter writer, float value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}