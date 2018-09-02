using Kaitai;
using System.IO;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class UInteger16Definition : NumberDefinition<ushort>
    {
        public UInteger16Definition()
        : base(RcpTypes.Datatype.Uint16)
        {
            FMinimum = ushort.MinValue;
            FMaximum = ushort.MaxValue;
            FMultipleOf = 1;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            DefaultChanged = Default != 0;

            MinimumChanged = Minimum != ushort.MinValue;
            MaximumChanged = Maximum != ushort.MaxValue;
            MultipleOfChanged = MultipleOf != 1;
        }

        public override ushort ReadValue(KaitaiStream input)
        {
            return input.ReadU2be();
        }

        public override void WriteValue(BinaryWriter writer, ushort value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}