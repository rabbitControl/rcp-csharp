using System;
using Kaitai;

using RCP.Protocol;
using System.IO;
using RCP.Exceptions;

namespace RCP.Parameter
{
    internal class UriParameter : ValueParameter<string>, IUriParameter
    {
        private bool FSchemaChanged;
        private string FSchema;
        public string Schema { get { return FSchema; } set { FSchema = value; FSchemaChanged = true; SetDirty(); } }

        private bool FFilterChanged;
        private string FFilter;
        public string Filter { get { return FFilter; } set { FFilter = value; FFilterChanged = true; SetDirty(); } }

        public UriParameter(Int16 id, IParameterManager manager) : 
            base (id, RcpTypes.Datatype.Uri, manager)
        {
            FValue = "";
            FDefault = "";
            FSchema = "file";
            FFilter = "";
        }

        protected override bool AnyChanged()
        {
            return base.AnyChanged() || FSchemaChanged || FFilterChanged;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != "";
            FDefaultChanged = Default != "";
            FSchemaChanged = FSchema != "";
            FFilterChanged = FFilter != "";
        }

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
