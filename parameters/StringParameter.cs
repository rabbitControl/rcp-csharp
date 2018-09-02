using System;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class StringParameter : ValueParameter<string>
    {
        public new StringDefinition Type => base.Type as StringDefinition;

        public StringParameter(Int16 id, IParameterManager manager, StringDefinition typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }

        public string RegularExpression
        {
            get => Type.RegularExpression;
            set => Type.RegularExpression = value;
        }
    }
}
