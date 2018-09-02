using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class Integer8Definition : NumberDefinition<sbyte>
    {
        public Integer8Definition()
        : base(RcpTypes.Datatype.Int8)
        {
            FMinimum = sbyte.MinValue;
            FMaximum = sbyte.MaxValue;
            FMultipleOf = 1;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0;

            MinimumChanged = Minimum != sbyte.MinValue;
            MaximumChanged = Maximum != sbyte.MaxValue;
            MultipleOfChanged = MultipleOf != 1;
        }

        public override sbyte ReadValue(KaitaiStream input)
        {
            return input.ReadS1();
        }

        public override void WriteValue(BinaryWriter writer, sbyte value)
        {
            writer.Write(value);
        }
    }
}