using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Integer32Definition : NumberDefinition<int>
    {
        public Integer32Definition()
        : base(RcpTypes.Datatype.Int32)
        {
            Minimum = int.MinValue;
            Maximum = int.MaxValue;
            MultipleOf = 1;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0;

            MinimumChanged = Minimum != int.MinValue;
            MaximumChanged = Maximum != int.MaxValue;
            MultipleOfChanged = MultipleOf != 1;
        }

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