using Kaitai;

using RCP.Protocol;
using System;
using System.IO;

namespace RCP.Parameter
{
    public class StringParameter : ValueParameter<string>
    {
        public StringParameter(Int16 id, IManager manager) : 
            base (id, RcpTypes.Datatype.String, manager)
        { }

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
