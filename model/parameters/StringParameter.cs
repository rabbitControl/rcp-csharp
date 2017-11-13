using Kaitai;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RCP.Model
{
    public class StringParameter : ValueParameter<string>
    {
        public new IStringDefinition TypeDefinition
        {
            get { return base.TypeDefinition as IStringDefinition; }
        }

        public StringParameter(uint id): 
            base (id, new StringDefinition())
        { }

        public StringParameter(uint id, IStringDefinition definition):
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
