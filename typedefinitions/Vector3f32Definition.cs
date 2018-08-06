using Kaitai;
using System.IO;

using RCP.Protocol;
using System.Numerics;

namespace RCP.Parameter
{
    public class Vector3f32Definition : NumberDefinition<Vector3>
    {
        public Vector3f32Definition()
        : base(RcpTypes.Datatype.Vector3f32)
        {
            Minimum = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Maximum = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            MultipleOf = new Vector3(0.01f, 0.01f, 0.01f);
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FDefaultChanged = Default != new Vector3(0, 0, 0);

            FMinimumChanged = Minimum != new Vector3(float.MinValue, float.MinValue, float.MinValue);
            FMaximumChanged = Maximum != new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            FMultipleOfChanged = MultipleOf != new Vector3(0.01f, 0.01f, 0.01f);
        }

        public override Vector3 ReadValue(KaitaiStream input)
        {
            return new Vector3(input.ReadF4be(), input.ReadF4be(), input.ReadF4be());
        }

        public override void WriteValue(BinaryWriter writer, Vector3 value)
        {
            writer.Write(value.X, ByteOrder.BigEndian);
            writer.Write(value.Y, ByteOrder.BigEndian);
            writer.Write(value.Z, ByteOrder.BigEndian);
        }
    }
}