using Kaitai;

namespace RCP.Model
{
    public class BooleanParameter : ValueParameter<bool>
    {
        public new IBooleanDefinition TypeDefinition
        {
            get { return base.TypeDefinition as IBooleanDefinition; }
        }

        public BooleanParameter(uint id): 
            base (id, new BooleanDefinition())
        { }

        public BooleanParameter(uint id, IBooleanDefinition definition):
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