using System;

using Kaitai;
using RCP.Protocol;

namespace RCP.Parameter
{
    public class BooleanParameter : ValueParameter<bool>
    {
        public new IBooleanDefinition TypeDefinition
        {
            get { return base.TypeDefinition as IBooleanDefinition; }
        }

        public BooleanParameter(Int16 id, IManager manager) : 
            base (id, new BooleanDefinition(), manager)
        { }

        public BooleanParameter(Int16 id, IBooleanDefinition definition, IManager manager) :
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
