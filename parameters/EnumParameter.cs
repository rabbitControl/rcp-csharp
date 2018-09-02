using System;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class EnumParameter : ValueParameter<string>
    {
        public new EnumDefinition Type => base.Type as EnumDefinition;

        public string[] Entries
        {
            get => Type.Entries;
            set => Type.Entries = value;
        }

        public bool MultiSelect
        {
            get => Type.MultiSelect;
            set => Type.MultiSelect = value;
        }

        public EnumParameter(Int16 id, IParameterManager manager, EnumDefinition typeDefinition)
            : base(id, manager, typeDefinition)
        {
        }
    }
}
