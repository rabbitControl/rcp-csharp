using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Float32Definition : NumberDefinition<float>
    {
        public Float32Definition()
        : base(RcpTypes.Datatype.Float32)
        { }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FDefaultChanged = Default != 0.0f;

            FMinimumChanged = Minimum != -99999.0f;
            FMaximumChanged = Maximum != 99999.0f;
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