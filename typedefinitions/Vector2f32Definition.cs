using Kaitai;
using System.IO;

using RCP.Protocol;
using System.Numerics;

namespace RCP.Parameter
{
    public class Vector2f32Definition : NumberDefinition<Vector2>
    {
        public Vector2f32Definition()
        : base(RcpTypes.Datatype.Vector2f32)
        {
            Minimum = new Vector2(float.MinValue, float.MinValue);
            Maximum = new Vector2(float.MaxValue, float.MaxValue);
            MultipleOf = new Vector2(0.01f, 0.01f);
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FDefaultChanged = Default != new Vector2(0, 0);

            FMinimumChanged = Minimum != new Vector2(float.MinValue, float.MinValue);
            FMaximumChanged = Maximum != new Vector2(float.MaxValue, float.MaxValue);
            FMultipleOfChanged = MultipleOf != new Vector2(0.01f, 0.01f);
        }

        public override Vector2 ReadValue(KaitaiStream input)
        {
            return new Vector2(input.ReadF4be(), input.ReadF4be());
        }

        public override void WriteValue(BinaryWriter writer, Vector2 value)
        {
            writer.Write(value.X, ByteOrder.BigEndian);
            writer.Write(value.Y, ByteOrder.BigEndian);
        }
    }
}