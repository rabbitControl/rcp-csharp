using System;

using Kaitai;
using RCP.Protocol;

namespace RCP.Parameter
{
    public class EnumParameter : ValueParameter<ushort>
    {
        public new IEnumDefinition TypeDefinition
        {
            get { return base.TypeDefinition as IEnumDefinition; }
        }

        public EnumParameter(Int16 id, IManager manager) : 
            base (id, new EnumDefinition(), manager)
        { }

        public EnumParameter(Int16 id, IEnumDefinition definition, IManager manager):
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
