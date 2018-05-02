using System;
using Kaitai;

using RCP.Protocol;
using System.IO;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public class UriParameter : ValueParameter<string>
    {
        private bool FSchemaChanged;
        private string FSchema;
        public string Schema { get { return FSchema; } set { FSchema = value; FSchemaChanged = true; } }

        private bool FFilterChanged;
        private string FFilter;
        public string Filter { get { return FFilter; } set { FFilter = value; FFilterChanged = true; } }

        public UriParameter(Int16 id, IManager manager) : 
            base (id, RcpTypes.Datatype.Uri, manager)
        { }

        public override string ReadValue(KaitaiStream input)
        {
            return new RcpTypes.LongString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.LongString.Write(value, writer);
        }

        protected override void WriteTypeDefinitionOptions(BinaryWriter writer)
        {
            base.WriteTypeDefinitionOptions(writer);

            if (FSchemaChanged)
            {
                writer.Write((byte)RcpTypes.UriOptions.Schema);
                writer.Write(Schema);
                FSchemaChanged = false;
            }

            if (FFilterChanged)
            {
                writer.Write((byte)RcpTypes.UriOptions.Filter);
                writer.Write(Filter);
                FFilterChanged = false;
            }
        }

        protected override bool HandleTypeDefinitionOption(KaitaiStream input, byte code)
        {
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
