using System;
using System.IO;
using System.Numerics;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Vector3f32Parameter : NumberParameter<Vector3>
    {
        public Vector3f32Parameter(Int16 id, IManager manager)
        : base(id, RcpTypes.Datatype.Vector3f32, manager)
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