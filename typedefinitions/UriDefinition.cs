using System;
using System.IO;
using Kaitai;

using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    public class UriDefinition : DefaultDefinition<string>, IUriDefinition
    {
        public bool SchemaChanged { get; private set; }
        private string FSchema;
        public string Schema { get { return FSchema; } set { SchemaChanged = FSchema != value; FSchema = value;} }

        public bool FilterChanged { get; private set; }
        private string FFilter;
        public string Filter { get { return FFilter; } set { FilterChanged = FFilter != value; FFilter = value; } }

        public UriDefinition()
        : base(RcpTypes.Datatype.Uri)
        { }

        public override bool AnyChanged()
        {
            return base.AnyChanged() || SchemaChanged || FilterChanged;
        }

        public override void ResetForInitialize()
        {
            SchemaChanged = FSchema != "";
            FilterChanged = FFilter != "";
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

            if (SchemaChanged)
            {
                writer.Write((byte)RcpTypes.UriOptions.Schema);
                WriteValue(writer, FSchema);
                SchemaChanged = false;
            }

            if (FilterChanged)
            {
                writer.Write((byte)RcpTypes.UriOptions.Filter);
                WriteValue(writer, FFilter);
                FilterChanged = false;
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

        public override void CopyFrom(ITypeDefinition other)
        {
            base.CopyFrom(other);

            var otherUri = other as IUriDefinition;

            if (otherUri.SchemaChanged)
                FSchema = otherUri.Schema;

            if (otherUri.FilterChanged)
                FFilter = otherUri.Filter;
        }
    }
}