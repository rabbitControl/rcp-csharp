using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Types
{
    public class Integer32Definition : NumberDefinition<int>
    {
        protected override int DefaultMinimum => int.MinValue;
        protected override int DefaultMaximum => int.MaxValue;
        protected override int DefaultMulitpleOf => 1;

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