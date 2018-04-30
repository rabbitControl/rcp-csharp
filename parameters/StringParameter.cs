using Kaitai;

using RCP.Protocol;
using System;

namespace RCP.Parameter
{
    public class StringParameter : ValueParameter<string>
    {
        public new IStringDefinition TypeDefinition
        {
            get { return base.TypeDefinition as IStringDefinition; }
        }

        public StringParameter(Int16 id, IManager manager) : 
            base (id, new StringDefinition(), manager)
        { }

        public StringParameter(Int16 id, IStringDefinition definition, IManager manager) :
            base(id, definition, manager)
        { }

        protected override bool HandleOption(KaitaiStream input, RcpTypes.ParameterOptions option)
        {
            switch (option)
            {
                case RcpTypes.ParameterOptions.Value:
                    Value = TypeDefinition.ReadValue(input);
                    return true;
            }

            return false;
        }
    }
}
