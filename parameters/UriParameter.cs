using System;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class UriParameter : ValueParameter<string>
    {
        public new UriDefinition Type => base.Type as UriDefinition;

        public UriParameter(Int16 id, IParameterManager manager, UriDefinition typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }

        public string Schema
        {
            get => Type.Schema;
            set => Type.Schema = value;
        }

        public string Filter
        {
            get => Type.Filter;
            set => Type.Filter = value;
        }
    }
}
