using System;
using System.IO;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{                           
    public class StringDefinition : DefaultDefinition<string>, IStringDefinition
    {
        public StringDefinition()
        : base(RcpTypes.Datatype.String)
        {
            FDefault = "";
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FDefaultChanged = Default != "";
        }

        public override string ReadValue(KaitaiStream input)
        {
            return new RcpTypes.LongString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.LongString.Write(value, writer);
        }
    }
}
