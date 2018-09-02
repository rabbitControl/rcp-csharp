using Kaitai;

using RCP.Protocol;
using System;
using System.IO;

namespace RCP.Parameter
{
    public sealed class StringParameter : ValueParameter<string>
    {
        public new StringDefinition TypeDefinition => base.TypeDefinition as StringDefinition;

        public StringParameter(Int16 id, IParameterManager manager, StringDefinition typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }

        public string RegularExpression
        {
            get { return TypeDefinition.RegularExpression; }
            set
            {
                TypeDefinition.RegularExpression = value;
                if (TypeDefinition.RegularExpressionChanged)
                    SetDirty();
            }
        }
    }
}
