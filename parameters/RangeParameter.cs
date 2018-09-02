using System;
using RCP.Types;

namespace RCP.Parameters
{
    public sealed class RangeParameter<T> : ValueParameter<Range<T>>, IRangeParameter where T : struct
    {
        public new RangeDefinition<T> Type => base.Type as RangeDefinition<T>;

        public RangeParameter(Int16 id, IParameterManager manager, RangeDefinition<T> typeDefinition)
            : base(id, manager, typeDefinition)
        {
        }

        IRangeDefinition IRangeParameter.Type => Type;
        object IRangeParameter.Lower { get => Value.Lower; set => Value = new Range<T>((T)value, Value.Upper); }
        object IRangeParameter.Upper { get => Value.Upper; set => Value = new Range<T>(Value.Lower, (T)value); }
    }
}
