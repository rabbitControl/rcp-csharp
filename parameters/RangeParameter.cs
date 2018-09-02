using System;

using Kaitai;
using RCP.Protocol;
using System.IO;
using RCP.Exceptions;
using System.Collections.Generic;

namespace RCP.Parameter
{
    public sealed class RangeParameter<T> : ValueParameter<Range<T>>, IRangeParameter where T : struct
    {
        public new RangeDefinition<T> TypeDefinition => base.TypeDefinition as RangeDefinition<T>;

        public RangeParameter(Int16 id, IParameterManager manager, RangeDefinition<T> typeDefinition)
            : base(id, manager, typeDefinition)
        {
        }

        IRangeDefinition IRangeParameter.TypeDefinition => TypeDefinition;
        object IRangeParameter.Lower { get => Value.Lower; set => Value = new Range<T>((T)value, Value.Upper); }
        object IRangeParameter.Upper { get => Value.Upper; set => Value = new Range<T>(Value.Lower, (T)value); }
    }
}
