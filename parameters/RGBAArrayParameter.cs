using System;
using System.Collections.Generic;
using System.IO;
using Kaitai;
using RCP;
using RCP.Protocol;
using RCP.Exceptions;
using System.Drawing;

namespace RCP.Parameter
{
    internal class RGBAArrayParameter : ArrayParameter<Color[], Color>, IRGBAArrayParameter
    {
        public RGBADefinition RGBADefinition => ArrayDefinition.ElementDefinition as RGBADefinition;

        public RGBAArrayParameter(Int16 id, IParameterManager manager, params int[] structure) : 
            base(id, RcpTypes.Datatype.String, manager, structure)
        {
            TypeDefinition = new ArrayDefinition<Color[], Color>(new RGBADefinition(), structure);
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
