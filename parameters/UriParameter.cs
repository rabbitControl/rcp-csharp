using System;
using Kaitai;

using RCP.Protocol;
using System.IO;

namespace RCP.Parameter
{
    public sealed class UriParameter : ValueParameter<string>
    {
        public new UriDefinition TypeDefinition => base.TypeDefinition as UriDefinition;

        public UriParameter(Int16 id, IParameterManager manager, UriDefinition typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }

        public string Schema
        {
            get { return TypeDefinition.Schema; }
            set
            {
                TypeDefinition.Schema = value;
                if (TypeDefinition.SchemaChanged)
                    SetDirty();
            }
        }

        public string Filter
        {
            get { return TypeDefinition.Filter; }
            set
            {
                TypeDefinition.Filter = value;
                if (TypeDefinition.FilterChanged)
                    SetDirty();
            }
        }
    }
}
