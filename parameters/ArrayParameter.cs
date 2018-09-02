using System;

using Kaitai;
using RCP.Protocol;
using System.IO;
using RCP.Exceptions;
using System.Collections.Generic;

namespace RCP.Parameter
{
    public sealed class ArrayParameter<T> : ValueParameter<T[]>
    {
        public new ArrayDefinition<T> TypeDefinition => base.TypeDefinition as ArrayDefinition<T>;

        public ArrayParameter(Int16 id, IParameterManager manager, ArrayDefinition<T> typeDefinition)
            : base(id, manager, typeDefinition)
        {
        }
    }
}
