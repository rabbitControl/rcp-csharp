using Kaitai;
using System.IO;

namespace RCP.Model
{
    public class Integer32Definition : NumberDefinition<int>
    {
        public Integer32Definition()
        : base(RcpTypes.Datatype.Int32)
        { }

        public override int ReadValue(KaitaiStream input)
        {
            return input.ReadS4be();
        }

        public override void WriteValue(BinaryWriter writer, int value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}