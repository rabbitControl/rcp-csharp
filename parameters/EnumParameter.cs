using System;

using Kaitai;
using RCP.Protocol;
using System.IO;
using RCP.Exceptions;
using System.Collections.Generic;

namespace RCP.Parameter
{
    internal class EnumParameter : ValueParameter<string>, IEnumParameter
    {
        public EnumDefinition EnumDefinition => TypeDefinition as EnumDefinition;

        public string[] Entries { get { return EnumDefinition.Entries; } set { EnumDefinition.Entries = value; SetDirty(); } }
        public bool MultiSelect { get { return EnumDefinition.MultiSelect; } set { EnumDefinition.MultiSelect = value; SetDirty(); } }

        public EnumParameter(Int16 id, IParameterManager manager) : 
            base (id, manager)
        {
            TypeDefinition = new EnumDefinition();
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
                EnumDefinition.WriteValue(writer, Value);
                FValueChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = EnumDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
