using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public class UriDefinition : DefaultDefinition<string>, IUriDefinition
    {
        private bool FSchemaChanged;
        private string FSchema;
        public string Schema { get { return FSchema; } set { FSchemaChanged = FSchema != value; FSchema = value;} }

        private bool FFilterChanged;
        private string FFilter;
        public string Filter { get { return FFilter; } set { FFilterChanged = FFilter != value; FFilter = value; } }

        public UriDefinition()
        : base(RcpTypes.Datatype.Uri)
        { }

        public override bool AnyChanged()
        {
            return FSchemaChanged || FFilterChanged;
        }

        public override void ResetForInitialize()
        {
            FSchemaChanged = FSchema != "";
            FFilterChanged = FFilter != "";
        }

        public override string ReadValue(KaitaiStream input)
        {
            return new RcpTypes.TinyString(input).Data;
        }

        public override void WriteValue(BinaryWriter writer, string value)
        {
            RcpTypes.TinyString.Write(value, writer);
        }

        protected override void WriteOptions(BinaryWriter writer)
        {
            base.WriteOptions(writer);

            if (FSchemaChanged)
            {
                writer.Write((byte)RcpTypes.UriOptions.Schema);
                WriteValue(writer, FSchema);
                FSchemaChanged = false;
            }

            if (FFilterChanged)
            {
                writer.Write((byte)RcpTypes.UriOptions.Filter);
                WriteValue(writer, FFilter);
                FFilterChanged = false;
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
                case RcpTypes.UriOptions.Default:
                    Default = ReadValue(input);
                    return true;

                case RcpTypes.UriOptions.Schema:
                    Schema = new RcpTypes.TinyString(input).Data;
                    return true;

                case RcpTypes.UriOptions.Filter:
                    Filter = new RcpTypes.TinyString(input).Data;
                    return true;
            }

            return false;
        }

        public override void CopyTo(ITypeDefinition other)
        {
            base.CopyTo(other);

            var otherUri = other as UriDefinition;

            if (FSchemaChanged)
                otherUri.Schema = FSchema;

            if (FFilterChanged)
                otherUri.Filter = FFilter;
        }
    }
}