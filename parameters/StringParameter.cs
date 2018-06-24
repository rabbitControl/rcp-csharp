using Kaitai;

using RCP.Protocol;
using System;
using System.IO;

namespace RCP.Parameter
{
    internal class StringParameter : ValueParameter<string>, IStringParameter
    {
        public StringDefinition StringDefinition => TypeDefinition as StringDefinition;

        public StringParameter(Int16 id, IParameterManager manager) : 
            base (id, manager)
        {
            TypeDefinition = new StringDefinition();

            FValue = "";
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            FValueChanged = FValue != "";
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (FValueChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                StringDefinition.WriteValue(writer, Value);
                FValueChanged = false;
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
