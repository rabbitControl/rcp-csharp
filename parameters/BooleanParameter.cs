using System;

using Kaitai;
using RCP.Protocol;
using System.IO;

namespace RCP.Parameter
{
    internal class BooleanParameter : ValueParameter<bool>, IBooleanParameter
    {
        public BooleanDefinition BooleanDefinition => TypeDefinition as BooleanDefinition;

        public BooleanParameter(Int16 id, IParameterManager manager) : 
            base (id, manager)
        {
            TypeDefinition = new BooleanDefinition();
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            ValueChanged = Value != false;
        }

        protected override void WriteValue(BinaryWriter writer)
        {
            if (ValueChanged)
            {
                writer.Write((byte)RcpTypes.ParameterOptions.Value);
                BooleanDefinition.WriteValue(writer, Value);
                ValueChanged = false;
            }
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = BooleanDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
