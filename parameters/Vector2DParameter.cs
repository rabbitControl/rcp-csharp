using System;
using System.IO;
using System.Numerics;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Vector2f32Parameter : NumberParameter<Vector2>
    {
        public Vector2f32Parameter(Int16 id, IManager manager)
        : base(id, RcpTypes.Datatype.Vector2f32, manager)
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