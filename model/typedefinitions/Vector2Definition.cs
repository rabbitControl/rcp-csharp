using System.IO;
using System.Numerics;
using Kaitai;

namespace RCP.Model
{
    public class Vector2f32Definition : NumberDefinition<Vector2>
    {
        public Vector2f32Definition()
        : base(RcpTypes.Datatype.Vector2f32)
        { }

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