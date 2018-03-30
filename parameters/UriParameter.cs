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

        public UriParameter(byte[] id): 
            base (id, new UriDefinition())
        { }

        public UriParameter(byte[] id, IUriDefinition definition):
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
