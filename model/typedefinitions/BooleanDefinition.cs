using System;
using System.IO;

namespace RCP.Model
{
    public class BooleanDefinition : DefaultDefinition<bool>
    {
        public BooleanDefinition()
        : base(RcpTypes.Datatype.Boolean)
        {
        }

        //protected override void writeproperties(binarywriter writer)
        //{
        //    writer.write((byte)rcptypes.booleanoptions.default);
        //    writer.write((byte)(default ? 1 : 0));
        //}

        public override void WriteValue(BinaryWriter writer, bool value)
        {
            writer.Write(value, ByteOrder.BigEndian);
        }
    }
}
