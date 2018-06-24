using System;
using Kaitai;

using RCP.Protocol;
using System.IO;

namespace RCP.Parameter
{
    internal class UriParameter : ValueParameter<string>, IUriParameter
    {
        public UriDefinition UriDefinition => TypeDefinition as UriDefinition;

        public string Schema { get { return UriDefinition.Schema; } set { UriDefinition.Schema = value; SetDirty(); } }
        public string Filter { get { return UriDefinition.Filter; } set { UriDefinition.Filter = value; SetDirty(); } }

        public UriParameter(Int16 id, IParameterManager manager) : 
            base (id, manager)
        {
            TypeDefinition = new UriDefinition();

            Value = "";
            Default = "";
            Schema = "file";
            Filter = "";
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = Value != "";
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (FValueChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                UriDefinition.WriteValue(writer, Value);
                FValueChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = UriDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
