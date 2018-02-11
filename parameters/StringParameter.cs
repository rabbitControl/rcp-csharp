using Kaitai;

using RCP.Protocol;

namespace RCP.Parameter
{
    public class StringParameter : ValueParameter<string>
    {
        public new IStringDefinition TypeDefinition
        {
            get { return base.TypeDefinition as IStringDefinition; }
        }

        public StringParameter(int id): 
            base (id, new StringDefinition())
        { }

        public StringParameter(int id, IStringDefinition definition):
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
