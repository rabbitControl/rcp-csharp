using System;
using System.IO;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Float32Parameter : NumberParameter<float>
    {
        public Float32Parameter(Int16 id, IManager manager)
        : base(id, RcpTypes.Datatype.Float32, manager)
        { }

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