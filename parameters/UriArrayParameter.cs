using System;
using System.Collections.Generic;
using System.IO;
using Kaitai;
using RCP;
using RCP.Protocol;
using RCP.Exceptions;

namespace RCP.Parameter
{
    internal class UriArrayParameter : ArrayParameter<string[], string>, IUriArrayParameter
    {
        public UriDefinition UriDefinition => ArrayDefinition.ElementDefinition as UriDefinition;

        public UriArrayParameter(Int16 id, IParameterManager manager, params int[] structure) : 
            base(id, RcpTypes.Datatype.String, manager, structure)
        {
            TypeDefinition = new ArrayDefinition<string[], string>(new UriDefinition(), structure);
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
