using System;
using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class UriParameter : ValueParameter<string>
    {
        public new IUriDefinition TypeDefinition
        {
            get { return base.TypeDefinition as IUriDefinition; }
        }

        public UriParameter(Int16 id, IManager manager) : 
            base (id, new UriDefinition(), manager)
        { }

        public UriParameter(Int16 id, IUriDefinition definition, IManager manager) :
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
