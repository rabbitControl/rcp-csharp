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

            FDefaultChanged = Default != 0;

            FMinimumChanged = Minimum != int.MinValue;
            FMaximumChanged = Maximum != int.MaxValue;
            FMultipleOfChanged = MultipleOf != 1;
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