using System;
using System.Collections.Generic;
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

        public T Minimum
        {
            get => Type.ElementType.Minimum;
            set
            {
                if (!Equals(value, Minimum))
                {
                    Type.ElementType.Minimum = value;
                    OnPropertyChanged();
                }
            }
        }

        public T Maximum
        {
            get => Type.ElementType.Maximum;
            set
            {
                if (!Equals(value, Maximum))
                {
                    Type.ElementType.Maximum = value;
                    OnPropertyChanged();
                }
            }
        }

        public T Lower
        {
            get => Value.Lower;
            set
            {
                if (!Equals(value, Lower))
                {
                    Value = new Range<T>(value, Upper);
                    OnPropertyChanged();
                }
            }
        }

        public T Upper
        {
            get => Value.Upper;
            set
            {
                if (!Equals(value, Upper))
                {
                    Value = new Range<T>(Lower, value);
                    OnPropertyChanged();
                }
            }
        }

        IRangeDefinition IRangeParameter.Type => Type;
        object IRangeParameter.Lower { get => Lower; set => Lower = (T)value; }
        object IRangeParameter.Upper { get => Upper; set => Upper = (T)value; }
    }
}
