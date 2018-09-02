using System;
using RCP.Protocol;
using RCP.Types;

namespace RCP.Parameters
{
    public class NumberParameter<T> : ValueParameter<T>
        where T : struct
    {
        public new NumberDefinition<T> Type => base.Type as NumberDefinition<T>;

        public NumberParameter(Int16 id, IParameterManager manager, NumberDefinition<T> typeDefinition) 
            : base(id, manager, typeDefinition)
        {
        }

        public T Minimum
        {
            get => Type.Minimum;
            set => Type.Minimum = value;
        }

        public T Maximum
        {
            get => Type.Maximum;
            set => Type.Maximum = value;
        }

        public T MultipleOf
        {
            get => Type.MultipleOf;
            set => Type.MultipleOf = value;
        }

        public RcpTypes.NumberScale Scale
        {
            get => Type.Scale;
            set => Type.Scale = value;
        }

        public string Unit
        {
            get => Type.Unit;
            set => Type.Unit = value;
        }
    }
}
