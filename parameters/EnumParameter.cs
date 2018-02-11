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

        public EnumParameter(int id): 
            base (id, new EnumDefinition())
        { }

        public EnumParameter(int id, IEnumDefinition definition):
            base(id, definition)
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
