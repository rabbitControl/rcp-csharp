using Kaitai;

using RCP.Protocol;
using System;
using System.IO;

namespace RCP.Parameter
{
    internal class StringParameter : ValueParameter<string>, IStringParameter
    {
        public StringDefinition StringDefinition => TypeDefinition as StringDefinition;

        public string RegularExpression { get { return StringDefinition.RegularExpression; } set { StringDefinition.RegularExpression = value; if (StringDefinition.RegularExpressionChanged) SetDirty(); } }

        public StringParameter(Int16 id, IParameterManager manager) : 
            base (id, manager)
        {
            TypeDefinition = new StringDefinition();

            FValue = "";
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            ValueChanged = FValue != "";
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (ValueChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                StringDefinition.WriteValue(writer, Value);
                ValueChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = StringDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
