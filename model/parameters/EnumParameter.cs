using Kaitai;

namespace RCP.Model
{
    public class EnumParameter : ValueParameter<ushort>
    {
        public new IEnumDefinition TypeDefinition
        {
            get { return base.TypeDefinition as IEnumDefinition; }
        }

        public EnumParameter(uint id): 
            base (id, new EnumDefinition())
        { }

        public EnumParameter(uint id, IEnumDefinition definition):
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
