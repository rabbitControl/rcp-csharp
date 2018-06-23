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
            Minimum = float.MinValue;
            Maximum = float.MaxValue;
            MultipleOf = 0.01f;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FDefaultChanged = Default != 0.0f;

            FMinimumChanged = Minimum != float.MinValue;
            FMaximumChanged = Maximum != float.MaxValue;
            FMultipleOfChanged = MultipleOf != 0.01f;
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