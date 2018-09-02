using System;

using Kaitai;
using RCP.Protocol;
using System.IO;
using RCP.Exceptions;
using System.Collections.Generic;

namespace RCP.Parameter
{
    public sealed class EnumParameter : ValueParameter<string>
    {
        public EnumDefinition EnumDefinition => TypeDefinition as EnumDefinition;

        public string[] Entries { get { return EnumDefinition.Entries; } set { EnumDefinition.Entries = value; if (EnumDefinition.EntriesChanged) SetDirty(); } }
        public bool MultiSelect { get { return EnumDefinition.MultiSelect; } set { EnumDefinition.MultiSelect = value; if (EnumDefinition.MultiSelectChanged) SetDirty(); } }

        public EnumParameter(Int16 id, IParameterManager manager, EnumDefinition typeDefinition)
            : base(id, manager, typeDefinition)
        {
        }
    }
}
