using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;
using System;

namespace RCP.Parameter
{
    public class UriDefinition : DefaultDefinition<string>, IUriDefinition
    {
        public string Schema { get; set; }
        public string Filter { get; set; }

        public UriDefinition()
        : base(RcpTypes.Datatype.Uri)
        { }

        public override string ReadValue(KaitaiStream input)
        {
            return new RcpTypes.LongString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.LongString.Write(value, writer);
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (!string.IsNullOrWhiteSpace(Schema))
            {
                writer.Write((byte)RcpTypes.UriOptions.Schema);
                writer.Write(Schema);
            }

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                writer.Write((byte)RcpTypes.UriOptions.Filter);
                writer.Write(Filter);
            }
        }

        protected override bool HandleOption(KaitaiStream input, byte code)
        {
            var result = base.HandleOption(input, code);
            if (result)
                return result;

            var option = (RcpTypes.UriOptions)code;
            if (!Enum.IsDefined(typeof(RcpTypes.UriOptions), option))
                throw new RCPDataErrorException("UriDefinition parsing: Unknown option: " + option.ToString());

            switch (option)
            {
                case RcpTypes.UriOptions.Schema:
                    Schema = new RcpTypes.TinyString(input).Data;
                    return true;

                case RcpTypes.UriOptions.Filter:
                    Filter = new RcpTypes.TinyString(input).Data;
                    return true;
            }

            return false;
        }
    }
}
