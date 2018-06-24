using System;
using System.Collections.Generic;
using System.IO;
using Kaitai;
using RCP;
using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    internal class StringArrayParameter<T> : ArrayParameter<T, string>, IStringArrayParameter<T>
    {
        public StringDefinition StringDefinition => ArrayDefinition.ElementDefinition as StringDefinition;

        public StringArrayParameter(Int16 id, IParameterManager manager, params int[] structure) : 
            base(id, RcpTypes.Datatype.String, manager, structure)
        {
            TypeDefinition = new ArrayDefinition<T, string>(new StringDefinition(), structure);
        }

        public override void ResetForInitialize()
        {
            base.ResetForInitialize();

            //FValueChanged = FValue != "";
        }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = ArrayDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
