using System;
using System.IO;
using System.Drawing;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    internal class RGBAParameter : ValueParameter<Color>, IRGBAParameter
    {
        public RGBADefinition RGBADefinition => TypeDefinition as RGBADefinition;

        public RGBAParameter(Int16 id, IParameterManager manager) : 
            base (id, manager)
        {
            TypeDefinition = new RGBADefinition();

            FValue = Color.Black;
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            ValueChanged = Value != Color.Black;
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (ValueChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                RGBADefinition.WriteValue(writer, Value);
                ValueChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = RGBADefinition.ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
