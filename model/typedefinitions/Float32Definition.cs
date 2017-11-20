using Kaitai;
using System.IO;

namespace RCP.Model
{
    public class Float32Definition : NumberDefinition<float>
    {
        public Float32Definition()
        : base(RcpTypes.Datatype.Float32)
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