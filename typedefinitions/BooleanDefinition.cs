using System.IO;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class BooleanDefinition : DefaultDefinition<bool>, IBooleanDefinition
    {
        public BooleanDefinition()
        : base(RcpTypes.Datatype.Boolean)
        { }

        public override bool ReadValue(KaitaiStream input)
        {
            return input.ReadU1() > 0;
        }

        public override void WriteValue(BinaryWriter writer, bool value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}
