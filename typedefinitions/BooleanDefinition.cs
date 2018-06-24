using System;
using System.IO;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{                           
    public class BooleanDefinition : DefaultDefinition<bool>, IBoolDefinition
    {
        public BooleanDefinition()
        : base(RcpTypes.Datatype.Boolean)
        {
            FDefault = false;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FDefaultChanged = Default != false;
        }

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
