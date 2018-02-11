using System.IO;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class StringDefinition : DefaultDefinition<string>, IStringDefinition
    {
        //public string Format { get; set; }
        //public string Filemask { get; set; }
        //public string MaxChars { get; set; }

        public StringDefinition()
        : base(RcpTypes.Datatype.String)
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
