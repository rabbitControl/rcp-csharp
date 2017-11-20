using System.IO;
using System.Numerics;
using Kaitai;

namespace RCP.Model
{
    public class Vector3f32Definition : NumberDefinition<Vector3>
    {
        public Vector3f32Definition()
        : base(RcpTypes.Datatype.Vector3f32)
        { }

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